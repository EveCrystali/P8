using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuide.Services.Interfaces
{
    public interface IRewardsService
    {
        void CalculateRewards(User user);
        double GetDistance(Locations loc1, Locations loc2);
        bool IsWithinAttractionProximity(Attraction attraction, Locations location);
        void SetDefaultProximityBuffer();
        void SetProximityBuffer(int proximityBuffer);
    }
}