using GpsUtil.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.Users;
using TourGuide.Utilities;

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
        User user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        Attraction? attraction = _fixture.GpsUtil.GetAttractions()[0];
        if (attraction == null) { Assert.Fail(); }
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        _fixture.TourGuideService.TrackUserLocation(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.Single(userRewards);
    }

    [Fact]
    public void IsWithinAttractionProximity()
    {
        var attraction = _fixture.GpsUtil.GetAttractions()[0];
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

    [Fact]
    public void NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        var user = _fixture.TourGuideService.GetAllUsers()[0];
        _fixture.RewardsService.CalculateRewards(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(_fixture.GpsUtil.GetAttractions().Count, userRewards.Count);
    }

}
