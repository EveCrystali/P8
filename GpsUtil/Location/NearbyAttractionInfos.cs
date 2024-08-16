using System;

namespace GpsUtil.Location;

public class NearbyAttractionInfos(string attractionName, 
    double attractionLatitude, double attractionLongitude, 
    double userLatitude, double userLongitude,
    double distanceInMiles, int rewardPoints)
{
    public string AttractionName { get; } = attractionName;
    public double AttractionLatitude { get; } = attractionLatitude;
    public double AttractionLongitude { get; } = attractionLongitude;
    public double UserLatitude { get; } = userLatitude;
    public double UserLongitude { get; } = userLongitude;
    public double DistanceInMiles { get; } = distanceInMiles;
    public int RewardPoints { get; } = rewardPoints;
}