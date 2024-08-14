using BenchmarkDotNet.Running;
using TourGuideBenchmark;

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<GetTripDealsBenchmark>();
    }
}