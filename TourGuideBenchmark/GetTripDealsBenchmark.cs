using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using TourGuide.Services;
using TourGuide.Users;
using TourGuideTest;

namespace TourGuideBenchmark
{
    public class GetTripDealsBenchmark
    {
        private TourGuideService _tourGuideService;
        private DependencyFixture _fixture;
        private User _user;

        [GlobalSetup]
        public void Setup()
        {
            _fixture = new DependencyFixture();
            _fixture.Initialize(0); // Initialiser la fixture

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TourGuideService>();

            _tourGuideService = new TourGuideService(
                logger,
                _fixture.GpsUtil,
                _fixture.RewardsService,
                loggerFactory);

            _user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        }

        [Benchmark]
        public void BenchmarkGetTripDeals()
        {
            // Act
            _tourGuideService.GetTripDeals(_user);
        }
    }
}
