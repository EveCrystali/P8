namespace TourGuide.LibrairiesWrappers.Interfaces
{
    public interface IRewardCentral
    {
        int GetAttractionRewardPoints(Guid attractionId, Guid userId);
    }
}