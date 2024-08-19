using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using TourGuide.Services;
using TourGuide.Users;
using TourGuideTest;

namespace TourGuideBenchmark.Helpers;

public class BenchmarkHelper
{
    public TourGuideService _tourGuideService;
    public DependencyFixture _fixture;
    public User _user;

    [GlobalSetup]
    public void Setup()
    {
        _fixture = new DependencyFixture();
        _fixture.Initialize(1000); // Initialiser la fixture

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<TourGuideService> logger = loggerFactory.CreateLogger<TourGuideService>();

        _tourGuideService = new TourGuideService(
            logger,
            _fixture.GpsUtil,
            _fixture.RewardsService,
            loggerFactory);

        _user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
    }
}
