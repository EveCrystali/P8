namespace RewardCentral;

public class RewardCentral
{
    public static int GetAttractionRewardPoints(Guid attractionId, Guid userId)
    {
        int randomDelay = new Random().Next(1, 1000);
        Thread.Sleep(randomDelay);

        int randomInt = new Random().Next(1, 1000);
        return randomInt;
    }
}