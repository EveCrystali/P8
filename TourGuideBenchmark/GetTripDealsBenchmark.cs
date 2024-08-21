using BenchmarkDotNet.Attributes;
using TourGuideBenchmark.Helpers;

namespace TourGuideBenchmark
{
    public class GetTripDealsBenchmark : BenchmarkHelper
    {
        [Benchmark]
        public void BenchmarkGetTripDeals()
        {
            _tourGuideService.GetTripDeals(_user);
        }
    }
}