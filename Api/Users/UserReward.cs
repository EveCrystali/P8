using GpsUtil.Location;

namespace TourGuide.Users;

public class UserReward(VisitedLocation visitedLocation, Attraction attraction, int rewardPoints)
{
    public VisitedLocation VisitedLocation { get; } = visitedLocation;
    public Attraction Attraction { get; } = attraction;
    public int RewardPoints { get; set; } = rewardPoints;

    public UserReward(VisitedLocation visitedLocation, Attraction attraction) : this(visitedLocation, attraction, 0)
    {
    }
}
