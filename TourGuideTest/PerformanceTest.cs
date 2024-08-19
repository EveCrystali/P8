using System.Diagnostics;
using GpsUtil.Location;
using TourGuide.Users;
using Xunit.Abstractions;

namespace TourGuideTest
{
    public class PerformanceTest : IClassFixture<DependencyFixture>
    {
        /*
         * Note on performance improvements:
         *
         * The number of generated users for high-volume tests can be easily adjusted using this method:
         *
         *_fixture.Initialize(100000); (for example)
         *
         *
         * These tests can be modified to fit new solutions, as long as the performance metrics at the end of the tests remain consistent.
         *
         * These are the performance metrics we aim to achieve:
         *
         * highVolumeTrackLocation: 100,000 users within 15 minutes:
         * Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
         *
         * highVolumeGetRewards: 100,000 users within 20 minutes:
         * Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        */

        private readonly DependencyFixture _fixture;

        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        // TODO: Un"skip" this test
        [Fact(Skip = ("Delete Skip when you want to pass the test"))]
        public void HighVolumeTrackLocation()
        {
            //On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(1000);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var user in allUsers)
            {
                _fixture.TourGuideService.TrackUserLocation(user);
            }
            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        // TODO: Un"skip" this test
        [Fact(Skip = ("Delete Skip when you want to pass the test"))]
        public void HighVolumeGetRewards()
        {
            //On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(10);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Attraction attraction = _fixture.GpsUtil.GetAttractions()[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            allUsers.ForEach(u => u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.Now)));

            allUsers.ForEach(u => _fixture.RewardsService.CalculateRewards(u));

            foreach (var user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0);
            }
            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }
    }
}