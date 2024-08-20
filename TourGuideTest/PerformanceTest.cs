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

        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            //On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(100000);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new();
            stopWatch.Start();

            foreach (User user in allUsers)
            {
                await _fixture.TourGuideService.TrackUserLocationAsync(user);
            }

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation(Number of users: {allUsers.Count}): Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        // DONE: Un"skip" this test
        [Fact]
        public async Task HighVolumeGetRewards()
        {
            //On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(100000);

            Stopwatch stopWatch = new();
            stopWatch.Start();

            Attraction attraction = (await _fixture.GpsUtil.GetAttractionsAsync())[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            allUsers.ForEach(u => u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.Now)));

            // Create a list of tasks to run CalculateRewardsAsync in parallel
            List<Task> tasks = [];
            allUsers.ForEach(u => tasks.Add(_fixture.RewardsService.CalculateRewardsAsync(u)));

            // Await for all tasks to be completed
            await Task.WhenAll(tasks);

            foreach (User user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0, $"User {user.UserName} has no rewards.");
            }

            stopWatch.Stop();

            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }
    }
}