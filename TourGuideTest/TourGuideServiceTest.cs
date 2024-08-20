using GpsUtil.Location;
using Microsoft.Extensions.Logging;
using TourGuide.Users;
using TripPricer;

namespace TourGuideTest
{
    public class TourGuideServiceTour : IClassFixture<DependencyFixture>
    {
        public ILogger _logger;
        private readonly DependencyFixture _fixture;

        public TourGuideServiceTour(DependencyFixture fixture)
        {
            _fixture = fixture;
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
            _logger = loggerFactory.CreateLogger<TourGuideServiceTour>();
        }

        [Fact]
        public async Task GetUserLocationAsync()
        {
            _fixture.Initialize(0);
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);
            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        [Fact]
        public void AddUser()
        {
            _fixture.Initialize(0);
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            User user2 = new(Guid.NewGuid(), "jon2", "000", "jon2@tourGuide.com");

            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            User retrievedUser = _fixture.TourGuideService.GetUser(user.UserName);
            User retrievedUser2 = _fixture.TourGuideService.GetUser(user2.UserName);

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user, retrievedUser);
            Assert.Equal(user2, retrievedUser2);
        }

        [Fact]
        public void GetAllUsers()
        {
            _fixture.Initialize(0);
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            User user2 = new(Guid.NewGuid(), "jon2", "000", "jon2@tourGuide.com");

            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Contains(user, allUsers);
            Assert.Contains(user2, allUsers);
        }

        [Fact]
        public async Task TrackUser()
        {
            _fixture.Initialize();
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        [Fact]
        public async Task GetNearbyAttractions()
        {
            // Arrange
            _fixture.Initialize(0);
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);
            user.AddToVisitedLocations(visitedLocation);

            // Act
            Attraction[] attractions = _fixture.TourGuideService.GetNearbyAttractions(visitedLocation);
            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(5, attractions.Length);
        }

        /// <summary>
        /// Test case for the GetTripDeals method of the TourGuideService class.
        /// This test case verifies that the GetTripDeals method returns the
        /// expected number of providers. The expected number is 10.
        /// </summary>
        [Fact]
        public void GetTripDeals()
        {
            // Arrange
            _fixture.Initialize(0); // Initialize the fixture
            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com"); // Create a test user

            // Act
            List<Provider> providers = _fixture.TourGuideService.GetTripDeals(user); // Get trip deals for the user
            _fixture.TourGuideService.Tracker.StopTracking(); // Stop tracking

            // Assert
            Assert.Equal(10, providers.Count); // Verify that the expected number of providers is returned
        }
    }
}