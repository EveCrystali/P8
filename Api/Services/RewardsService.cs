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

    /*
    * This method was not thread safe because using count++ operator whereas count is declared as static.
    * The issue is that the count variable is declared as static, which means it belongs to the class itself,
    * not to any instance of the class. However, it is being updated from an instance method CalculateRewards.
    * In C#, when a static field is updated from an instance method, it can lead to unexpected behavior,
    * especially in multi-threaded environments. This is because static fields are shared across all instances of the class,
    * and updating it from an instance method can cause conflicts between different instances. */

    // OPTIMIZE: Maybe this could be optimize but not sure (test NearAllAttractions test is 14.0sec)
    /// <summary>
    /// Calculates the rewards for a given user.
    /// </summary>
    /// <param name="user">The user.</param>
    public void CalculateRewards(User user)
    {
        // Increment the count of rewards calculations
        Interlocked.Increment(ref count);

        // Set var copy to avoid System.InvalidOperationException : Collection was modified; enumeration operation may not execute."
        List<VisitedLocation> userVisitedLocations = new(user.VisitedLocations);
        List<Attraction> getAllAttractions = _gpsUtil.GetAttractions();
        // Create a hash set of existing reward attraction names to avoid duplicates
        HashSet<string> existingRewardAttractions = new(user.UserRewards.Select(r => r.Attraction.AttractionName));

        // Create a list to store the new rewards to add to the user at the end
        List<UserReward> rewardsToAdd = [];

        foreach (VisitedLocation visitedLocation in userVisitedLocations)
        {
            foreach (Attraction attraction in getAllAttractions)
            {
                // Check if the attraction is not already a reward and if the user is near the attraction
                if (!existingRewardAttractions.Contains(attraction.AttractionName) && NearAttraction(visitedLocation, attraction))
                {
                    // Create a new UserReward
                    UserReward newReward = new(visitedLocation, attraction, GetRewardPoints(attraction, user));
                    // Add the reward to the list of rewards to add
                    rewardsToAdd.Add(newReward);

                    // Add the attraction to the list of existing rewards to avoid duplicates
                    existingRewardAttractions.Add(attraction.AttractionName);
                }
            }
        }

        // Add the new rewards to the user's rewards
        user.UserRewards.AddRange(rewardsToAdd);
    }

    public bool UserRewardsExist(string userRewardAttractionName, string attractionName)
    {
        List<Attraction> attractionList = _gpsUtil.GetAttractions();
        return attractionList.Exists(a => a.AttractionName == attractionName);
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
        double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dlon / 2) * Math.Sin(dlon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Return the distance in kilometers
        return EarthRadiusKm * c;
    }
}