﻿using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers;

public class GpsUtilWrapper : IGpsUtil
{
    private readonly GpsUtil.GpsUtil _gpsUtil;

    public GpsUtilWrapper()
    {
        _gpsUtil = new();
    }

    public async Task<VisitedLocation> GetUserLocationAsync(Guid userId)
    {
        return await GpsUtil.GpsUtil.GetUserLocationAsync(userId);
    }

    public async Task<List<Attraction>> GetAttractionsAsync()
    {
        return await GpsUtil.GpsUtil.GetAttractionsAsync();
    }
}