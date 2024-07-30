namespace TourGuide.Utilities;

public static class InternalTestHelper
{
    // Default value but can be more during test
    private static int internalUserNumber = 100;

    public static void SetInternalUserNumber(int number)
    {
        internalUserNumber = number;
    }

    public static int GetInternalUserNumber()
    {
        return internalUserNumber;
    }
}
