using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers
{
    public class RewardCentralWrapper : IRewardCentral
    {
        private readonly RewardCentral.RewardCentral _rewardCentral;

        public RewardCentralWrapper()
        {
            _rewardCentral = new();
        }

        public int GetAttractionRewardPoints(Guid attractionId, Guid userId)
        {
            return RewardCentral.RewardCentral.GetAttractionRewardPoints(attractionId, userId);
        }
    }
}
