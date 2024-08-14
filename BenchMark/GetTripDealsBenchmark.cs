using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using TourGuide.Services;
using TripPricer;
using TourGuide.Users;
using GpsUtil;
using Microsoft.Extensions.Logging;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.LibrairiesWrappers;
using GpsUtil.Location;

namespace TourGuideBenchmark;

public class GetTripDealsBenchmark
{
    private readonly TourGuideService _tourGuideService;
    private readonly User _testUser;

    public GetTripDealsBenchmark()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TourGuideService>();

        IGpsUtil gpsUtil = new GpsUtilWrapper();
        IRewardCentral rewardCentral = new RewardCentralWrapper();
        IRewardsService rewardsService = new RewardsService(gpsUtil, rewardCentral);

        _tourGuideService = new TourGuideService(logger, gpsUtil, rewardsService, loggerFactory);

        // Initialiser un utilisateur de test
        _testUser = new User(Guid.NewGuid(), "testUser", "000", "testUser@tourGuide.com");
        _testUser.UserPreferences.NumberOfAdults = 2;
        _testUser.UserPreferences.NumberOfChildren = 2;
        _testUser.UserPreferences.TripDuration = 5;

        var location = new Locations(10.0, 20.0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var visitedLocation = new VisitedLocation(user.UserId, location, DateTime.UtcNow); // Exemple de VisitedLocation
        var attraction = new Attraction("AttractionName", "City", "State", 10.0, 20.0); // Exemple d'Attraction
        UserReward userReward = new UserReward(visitedLocation, attraction, 10);

        _testUser.AddUserReward(userReward);
    }

    [Benchmark]
    public void BenchmarkGetTripDeals()
    {
        _tourGuideService.GetTripDeals(_testUser);
    }
}


