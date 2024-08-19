namespace TripPricer.Helpers;

internal static class ThreadLocalRandom
{
    private static readonly ThreadLocal<Random> threadLocal = new ThreadLocal<Random>(() => new Random());

    public static Random Current => threadLocal.Value;

    public static double NextDouble(double minValue, double maxValue)
    {
        return Current.NextDouble() * (maxValue - minValue) + minValue;
    }

    public static int Next(int minValue, int maxValue)
    {
        return Current.Next(minValue, maxValue);
    }
}