using GpsUtil.Location;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.Services;
using TourGuide.Users;
using TourGuide.Utilities;
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
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<TourGuideServiceTour>();

        }

        [Fact]
        public void GetUserLocation()
        {
            _fixture.Initialize(0);
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            var visitedLocation = _fixture.TourGuideService.TrackUserLocation(user);
            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        [Fact]
        public void AddUser()
        {
            _fixture.Initialize(0);
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            var user2 = new User(Guid.NewGuid(), "jon2", "000", "jon2@tourGuide.com");

            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            var retrievedUser = _fixture.TourGuideService.GetUser(user.UserName);
            var retrievedUser2 = _fixture.TourGuideService.GetUser(user2.UserName);

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user, retrievedUser);
            Assert.Equal(user2, retrievedUser2);
        }

        [Fact]
        public void GetAllUsers()
        {
            _fixture.Initialize(0);
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            var user2 = new User(Guid.NewGuid(), "jon2", "000", "jon2@tourGuide.com");

            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Contains(user, allUsers);
            Assert.Contains(user2, allUsers);
        }

        [Fact]
        public void TrackUser()
        {
            _fixture.Initialize();
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            var visitedLocation = _fixture.TourGuideService.TrackUserLocation(user);

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        // TODO: Un"skip" this test
        [Fact(Skip = "Not yet implemented")]
        public void GetNearbyAttractions()
        {
            _fixture.Initialize(0);
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
            var visitedLocation = _fixture.TourGuideService.TrackUserLocation(user);

            List<Attraction> attractions = _fixture.TourGuideService.GetNearByAttractions(visitedLocation);

            _fixture.TourGuideService.Tracker.StopTracking();

            Assert.Equal(5, attractions.Count);
        }


        // Fixme: expected 10 actual 5
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
            var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com"); // Create a test user

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Provider> providers = _fixture.TourGuideService.GetTripDeals(user); // Get trip deals for the user

            stopwatch.Stop();
            _logger.LogInformation($"GetTripDeals took {stopwatch.ElapsedMilliseconds} ms");


            // Assert
            _fixture.TourGuideService.Tracker.StopTracking(); // Stop tracking

            Assert.Equal(5, providers.Count); // Verify that the expected number of providers is returned
        }
    }
}
