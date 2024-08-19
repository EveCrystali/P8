using System.Diagnostics;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Utilities;

public class Tracker
{
    private readonly ILogger<Tracker> _logger;
    private static readonly TimeSpan TrackingPollingInterval = TimeSpan.FromMinutes(5);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITourGuideService _tourGuideService;

    public Tracker(ITourGuideService tourGuideService, ILogger<Tracker> logger)
    {
        _tourGuideService = tourGuideService;
        _logger = logger;
        Task.Run(() => Run(), _cancellationTokenSource.Token);
    }

    // Assures to shut down the Tracker thread
    public void StopTracking()
    {
        _cancellationTokenSource.Cancel();
    }

    public async Task Run()
    {
        Stopwatch stopwatch = new();

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            List<User> users = _tourGuideService.GetAllUsers();
            _logger.LogDebug($"Begin Tracker. Tracking {users.Count} users.");

            stopwatch.Start();

            users.ForEach(u => _tourGuideService.TrackUserLocation(u));

            stopwatch.Stop();

            _logger.LogDebug($"Tracker Time Elapsed: {stopwatch.ElapsedMilliseconds / 1000.0} seconds.");

            stopwatch.Reset();

            try
            {
                _logger.LogDebug("Tracker sleeping");
                await Task.Delay(TrackingPollingInterval, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        _logger.LogDebug("Tracker stopping");
    }
}