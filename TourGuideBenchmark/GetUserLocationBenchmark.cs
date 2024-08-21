using BenchmarkDotNet.Attributes;
using TourGuideBenchmark.Helpers;

namespace TourGuideBenchmark
{
    public class GetUserLocationBenchmark : BenchmarkHelper
    {
        [Benchmark]
        public void BenchmarkGetUserLocation()
        {
            _tourGuideService.GetUserLocationAsync(_user);
        }
    }
}