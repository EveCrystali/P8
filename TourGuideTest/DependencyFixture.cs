using Microsoft.Extensions.Logging;
using TourGuide.LibrairiesWrappers;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services;
using TourGuide.Services.Interfaces;
using TourGuide.Utilities;

namespace TourGuideTest
{
    public class DependencyFixture
    {
        public DependencyFixture()
        {
            Initialize();
        }

        public void Cleanup()
        {
            Initialize();
        }

        public void Initialize(int internalUserNumber = 100)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger<TourGuideService> tourGuideLogger = loggerFactory.CreateLogger<TourGuideService>();

            InternalTestHelper.SetInternalUserNumber(internalUserNumber);

            RewardCentral = new RewardCentralWrapper();
            GpsUtil = new GpsUtilWrapper();
            RewardsService = new RewardsService(GpsUtil, RewardCentral);
            TourGuideService = new TourGuideService(tourGuideLogger, GpsUtil, RewardsService, loggerFactory);
        }

        public IRewardCentral RewardCentral { get; set; }
        public IGpsUtil GpsUtil { get; set; }
        public IRewardsService RewardsService { get; set; }
        public ITourGuideService TourGuideService { get; set; }
    }
}