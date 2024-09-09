using BenchmarkDotNet.Running;
using TourGuideBenchmark;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkDotNet.Reports.Summary summary1 = BenchmarkRunner.Run<GetTripDealsBenchmark>();
        BenchmarkDotNet.Reports.Summary summary2 = BenchmarkRunner.Run<GetUserLocationBenchmark>();
    }
}