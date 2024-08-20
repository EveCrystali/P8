using GpsUtil.Location;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services.Interfaces
{
    public interface ITourGuideService
    {
        Tracker Tracker { get; }

        void AddUser(User user);

        List<User> GetAllUsers();

        Attraction[] GetNearbyAttractions(VisitedLocation visitedLocation);

        List<Provider> GetTripDeals(User user);

        User GetUser(string userName);

        Task<VisitedLocation> GetUserLocationAsync(User user);

        List<UserReward> GetUserRewards(User user);

        Task<VisitedLocation> TrackUserLocationAsync(User user);
    }
}