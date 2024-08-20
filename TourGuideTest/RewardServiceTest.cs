using GpsUtil.Location;
using Microsoft.Extensions.Logging;
using TourGuide.Users;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    private readonly ILogger<RewardServiceTest> _logger;


    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug);
        });
        _logger = loggerFactory.CreateLogger<RewardServiceTest>();
    }

    [Fact]
    public void UserGetRewards()
    {
        _fixture.Initialize(0);
        User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        Attraction? attraction = _fixture.GpsUtil.GetAttractions()[0];
        if (attraction == null) { Assert.Fail(); }
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        _fixture.TourGuideService.TrackUserLocationAsync(user);
        List<UserReward> userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.Single(userRewards);
    }

    [Fact]
    public void IsWithinAttractionProximity()
    {
        Attraction attraction = _fixture.GpsUtil.GetAttractions()[0];
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

    [Fact]
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        User user = _fixture.TourGuideService.GetAllUsers()[0];
        await _fixture.RewardsService.CalculateRewardsAsync(user);
        List<UserReward> userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(_fixture.GpsUtil.GetAttractions().Count, userRewards.Count);
    }

    [Fact]
    public async Task NearAllAttractionsInLoop()
    {
        int numberOfFail = 0;
        for (int i = 0; i < 10; i++)
        {

            try
            {
                _logger.LogDebug($"Loop {i}");
                await Task.FromResult(NearAllAttractions());
                _logger.LogInformation($"Success {i}");
            }

            catch
            {
                numberOfFail++;
                _logger.LogCritical($"Fail number {numberOfFail}");
            }
        }

        Assert.Equal(0, numberOfFail);
    }
}
