using GpsUtil.Location;

namespace TourGuide.LibrairiesWrappers.Interfaces
{
    public interface IGpsUtil
    {
        Task<VisitedLocation> GetUserLocationAsync(Guid userId);

        Task<List<Attraction>> GetAttractionsAsync();
    }
}