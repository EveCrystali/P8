using GpsUtil.Helpers;
using GpsUtil.Location;

namespace GpsUtil;

public class GpsUtil
{
    // NOTE: This line of code creates a `SemaphoreSlim` object named `rateLimiter` that limits the number of concurrent accesses to a resource to prevent overloading and excessive usage.
    private static readonly SemaphoreSlim rateLimiter = new(1000, 1000);

    // OPTIMIZE: HighVolumeTrackLocation
    // FIXME: Need optimization here - Must be able to handle 100 000 UserLocations in less than 15 minutes.
    // TODO: GOOD FIRST ISSUE
    /// <summary>
    /// Gets the user location.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The user location.</returns>
    public static async Task<VisitedLocation> GetUserLocationAsync(Guid userId)
    {
        // Limit the number of concurrent requests to prevent overloading and excessive usage
        rateLimiter.Wait();
        try
        {
            // Generate a random longitude between -180 and 180
            double longitude = ThreadLocalRandom.NextDouble(-180.0, 180.0);
            longitude = Math.Round(longitude, 6);

            // Generate a random latitude between -90 and 90
            double latitude = ThreadLocalRandom.NextDouble(-90, 90);
            latitude = Math.Round(latitude, 6);

            // Create a new VisitedLocation object with the user id, location, and current time
            VisitedLocation visitedLocation = new(userId, new Locations(latitude, longitude), DateTime.UtcNow);

            // Return the visited location
            return await Task.FromResult(visitedLocation);
        }
        finally
        {
            // Release the semaphore to allow other requests to access the resource
            rateLimiter.Release();
        }
    }

    public static List<Attraction> GetAttractions()
    {
        rateLimiter.Wait();

        try
        {
            // HACK: line below "Sleep" must be removed to make it faster
            SleepLighter();

            List<Attraction> attractions =
            [
                new Attraction("Disneyland", "Anaheim", "CA", 33.817595, -117.922008),
                new Attraction("Jackson Hole", "Jackson Hole", "WY", 43.582767, -110.821999),
                new Attraction("Mojave National Preserve", "Kelso", "CA", 35.141689, -115.510399),
                new Attraction("Joshua Tree National Park", "Joshua Tree National Park", "CA", 33.881866, -115.90065),
                new Attraction("Buffalo National River", "St Joe", "AR", 35.985512, -92.757652),
                new Attraction("Hot Springs National Park", "Hot Springs", "AR", 34.52153, -93.042267),
                new Attraction("Kartchner Caverns State Park", "Benson", "AZ", 31.837551, -110.347382),
                new Attraction("Legend Valley", "Thornville", "OH", 39.937778, -82.40667),
                new Attraction("Flowers Bakery of London", "Flowers Bakery of London", "KY", 37.131527, -84.07486),
                new Attraction("McKinley Tower", "Anchorage", "AK", 61.218887, -149.877502),
                new Attraction("Flatiron Building", "New York City", "NY", 40.741112, -73.989723),
                new Attraction("Fallingwater", "Mill Run", "PA", 39.906113, -79.468056),
                new Attraction("Union Station", "Washington D.C.", "CA", 38.897095, -77.006332),
                new Attraction("Roger Dean Stadium", "Jupiter", "FL", 26.890959, -80.116577),
                new Attraction("Texas Memorial Stadium", "Austin", "TX", 30.283682, -97.732536),
                new Attraction("Bryant-Denny Stadium", "Tuscaloosa", "AL", 33.208973, -87.550438),
                new Attraction("Tiger Stadium", "Baton Rouge", "LA", 30.412035, -91.183815),
                new Attraction("Neyland Stadium", "Knoxville", "TN", 35.955013, -83.925011),
                new Attraction("Kyle Field", "College Station", "TX", 30.61025, -96.339844),
                new Attraction("San Diego Zoo", "San Diego", "CA", 32.735317, -117.149048),
                new Attraction("Zoo Tampa at Lowry Park", "Tampa", "FL", 28.012804, -82.469269),
                new Attraction("Franklin Park Zoo", "Boston", "MA", 42.302601, -71.086731),
                new Attraction("El Paso Zoo", "El Paso", "TX", 31.769125, -106.44487),
                new Attraction("Kansas City Zoo", "Kansas City", "MO", 39.007504, -94.529625),
                new Attraction("Bronx Zoo", "Bronx", "NY", 40.852905, -73.872971),
                new Attraction("Cinderella Castle", "Orlando", "FL", 28.419411, -81.5812)
            ];

            return attractions;
        }
        finally
        {
            rateLimiter.Release();
        }
    }

    private static void SleepLighter()
    {
        Thread.Sleep(10);
    }
}