using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void UserGetRewards()
    {
        _fixture.Initialize(0);
        User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        Attraction? attraction = _fixture.GpsUtil.GetAttractions()[0];
        if (attraction == null) { Assert.Fail(); }
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        _fixture.TourGuideService.TrackUserLocation(user);
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
    public void NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        User user = _fixture.TourGuideService.GetAllUsers()[0];
        _fixture.RewardsService.CalculateRewards(user);
        List<UserReward> userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(_fixture.GpsUtil.GetAttractions().Count, userRewards.Count);
    }
}