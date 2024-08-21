using System.Collections.Concurrent;
using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;

    // NOTE : was previously 200 miles, is now the circumference of the earth
    private readonly double _attractionProximityRange = 40075;

    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;
    private static readonly object lockObject = new();

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    /// <summary>
    /// Calculates the rewards for a given user.
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task CalculateRewardsAsync(User user)
    {
        Interlocked.Increment(ref count);

        HashSet<VisitedLocation> userVisitedLocations = new(user.VisitedLocations);
        HashSet<Attraction> getAllAttractions = [.. (await _gpsUtil.GetAttractionsAsync())];
        HashSet<string> existingRewardAttractions = new(user.UserRewards.Select(r => r.Attraction.AttractionName));

        ConcurrentBag<UserReward> rewardsToAdd = [];
        await Parallel.ForEachAsync(userVisitedLocations, async (visitedLocation, token) =>
        {
            foreach (Attraction attraction in getAllAttractions)
            {
                lock (lockObject)
                {
                    if (!existingRewardAttractions.Contains(attraction.AttractionName) && NearAttraction(visitedLocation, attraction))
                    {
                        UserReward newReward = new(visitedLocation, attraction, GetRewardPoints(attraction, user));
                        rewardsToAdd.Add(newReward);
                        existingRewardAttractions.Add(attraction.AttractionName);
                    }
                }
            }
        });
        lock (lockObject)
        {
            user.UserRewards.AddRange(rewardsToAdd);
        }
    }

    /// <summary>
    /// Checks if a given location is within the proximity of a given attraction (= earth circumference). It enables to check that GetDistance returns a reasonable value.
    /// </summary>
    /// <param name="attraction">The attraction to check against.</param>
    /// <param name="location">The location to check for proximity.</param>
    /// <returns>True if the location is within the proximity of the attraction, otherwise false.</returns>
    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine($"Distance from {location.Latitude} {location.Longitude} to {attraction.AttractionName}: {GetDistance(attraction, location)}");

        // Return true if the distance is within the proximity range, otherwise false
        // NOTE: _attractionProximityRange was previously 200 miles, is now implemented to check if it is less than a lap of the earth.
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    private int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    /// <summary>
    /// Calculates the distance between two locations on the Earth's surface, in kilometers.
    /// </summary>
    /// <param name="loc1">The first location.</param>
    /// <param name="loc2">The second location.</param>
    /// <returns>The distance between the two locations, in kilometers.</returns>
    public double GetDistance(Locations loc1, Locations loc2)
    {
        const double EarthRadiusKm = 6371.0;

        // Convert the latitude and longitude from degrees to radians
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        // Calculate the differences in latitude and longitude
        double dlat = lat2 - lat1;
        double dlon = lon2 - lon1;

        // Calculate the Haversine distance formula
        double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) +
                   (Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dlon / 2) * Math.Sin(dlon / 2));

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Return the distance in kilometers
        return EarthRadiusKm * c;
    }
}