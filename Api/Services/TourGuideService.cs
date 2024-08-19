using System.Globalization;
using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services;

public class TourGuideService : ITourGuideService
{
    private readonly ILogger _logger;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardsService _rewardsService;
    private readonly TripPricer.TripPricer _tripPricer;
    public Tracker Tracker { get; private set; }
    private readonly Dictionary<string, User> _internalUserMap = new();
    private const string TripPricerApiKey = "test-server-api-key";
    private bool _testMode = true;

    public TourGuideService(ILogger<TourGuideService> logger, IGpsUtil gpsUtil, IRewardsService rewardsService, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _tripPricer = new();
        _gpsUtil = gpsUtil;
        _rewardsService = rewardsService;

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        if (_testMode)
        {
            _logger.LogInformation("TestMode enabled");
            _logger.LogDebug("Initializing users");
            InitializeInternalUsers();
            _logger.LogDebug("Finished initializing users");
        }

        var trackerLogger = loggerFactory.CreateLogger<Tracker>();

        Tracker = new Tracker(this, trackerLogger);
        AddShutDownHook();
    }

    public List<UserReward> GetUserRewards(User user)
    {
        return user.UserRewards;
    }

    public VisitedLocation GetUserLocation(User user)
    {
        return user.VisitedLocations.Any() ? user.GetLastVisitedLocation() : TrackUserLocation(user);
    }

    public User GetUser(string userName)
    {
        return _internalUserMap.ContainsKey(userName) ? _internalUserMap[userName] : null;
    }

    public List<User> GetAllUsers()
    {
        return _internalUserMap.Values.ToList();
    }

    public void AddUser(User user)
    {
        if (!_internalUserMap.ContainsKey(user.UserName))
        {
            _internalUserMap.Add(user.UserName, user);
        }
    }

    /// <summary>
    /// Retrieves trip deals for a user.
    /// </summary>
    /// <param name="user">The user for whom to retrieve trip deals.</param>
    /// <returns>A list of providers for the trip deals.</returns>
    public List<Provider> GetTripDeals(User user)
    {
        // Calculate the cumulative reward points of the user
        int cumulativeRewardPoints = user.UserRewards.Sum(i => i.RewardPoints);

        // Retrieve the trip prices using the TripPricer API
        List<Provider> providers = _tripPricer.GetPrice(TripPricerApiKey, user.UserId,
            user.UserPreferences.NumberOfAdults, user.UserPreferences.NumberOfChildren,
            user.UserPreferences.TripDuration, cumulativeRewardPoints);

        // Store the trip deals in the user object
        user.TripDeals = providers;

        return providers;
    }

    public VisitedLocation TrackUserLocation(User user)
    {
        VisitedLocation visitedLocation = _gpsUtil.GetUserLocation(user.UserId);
        user.AddToVisitedLocations(visitedLocation);
        _rewardsService.CalculateRewards(user);
        return visitedLocation;
    }

    /// <summary>
    /// Retrieves an sorted array of 5 maximum nearby attractions for a given visited location.
    /// </summary>
    /// <param name="visitedLocation">The location to retrieve nearby attractions for.</param>
    /// <returns>An array of nearby attractions.</returns>
    public Attraction[] GetNearbyAttractions(VisitedLocation visitedLocation)
    {
        // Get all attractions from the GpsUtil
        List<Attraction> nearbyAttractions = _gpsUtil.GetAttractions()
            .Where(attraction => _rewardsService.IsWithinAttractionProximity(attraction, visitedLocation.Location))
            .ToList();

        // Sort the nearby attractions by their distance from the visited location
        nearbyAttractions.Sort((Attraction a, Attraction b) =>
            _rewardsService.GetDistance(a, visitedLocation.Location).CompareTo(
            _rewardsService.GetDistance(b, visitedLocation.Location))
        );

        // Determine the count of nearby attractions to return
        int count = Math.Min(5, nearbyAttractions.Count);

        // Create an array to store the sorted nearby attractions
        Attraction[] arrayOfSortedNearbyAttractions = new Attraction[count];

        // Populate the array with the nearest nearby attractions
        for (int i = 0; i < count; i++)
        {
            arrayOfSortedNearbyAttractions[i] = nearbyAttractions[i];
        }

        return arrayOfSortedNearbyAttractions;
    }

    private void AddShutDownHook()
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => Tracker.StopTracking();
    }

    /**********************************************************************************
    *
    * Methods Below: For Internal Testing
    *
    **********************************************************************************/

    private void InitializeInternalUsers()
    {
        for (int i = 0; i < InternalTestHelper.GetInternalUserNumber(); i++)
        {
            var userName = $"internalUser{i}";
            var user = new User(Guid.NewGuid(), userName, "000", $"{userName}@tourGuide.com");
            GenerateUserLocationHistory(user);
            _internalUserMap.Add(userName, user);
        }

        _logger.LogDebug($"Created {InternalTestHelper.GetInternalUserNumber()} internal test users.");
    }

    private void GenerateUserLocationHistory(User user)
    {
        for (int i = 0; i < 3; i++)
        {
            var visitedLocation = new VisitedLocation(user.UserId, new Locations(GenerateRandomLatitude(), GenerateRandomLongitude()), GetRandomTime());
            user.AddToVisitedLocations(visitedLocation);
        }
    }

    private static readonly Random random = new Random();

    private double GenerateRandomLongitude()
    {
        return new Random().NextDouble() * (180 - (-180)) + (-180);
    }

    private double GenerateRandomLatitude()
    {
        return new Random().NextDouble() * (90 - (-90)) + (-90);
    }

    private DateTime GetRandomTime()
    {
        return DateTime.UtcNow.AddDays(-new Random().Next(30));
    }
}