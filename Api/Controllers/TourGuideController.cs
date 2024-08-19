using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;
    private readonly IRewardsService _rewardsService;
    private readonly IRewardCentral _rewardCentral;

    public TourGuideController(ITourGuideService tourGuideService, IRewardsService rewardsService, IRewardCentral rewardCentral)
    {
        _tourGuideService = tourGuideService;
        _rewardsService = rewardsService;
        _rewardCentral = rewardCentral;
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    /// <summary>
    /// Retrieves a json object with an array of 5 elements maximum containing the information below:
    /// - The name of the attraction
    /// - The latitude of the attraction
    /// - The longitude of the attraction
    /// - The latitude of the user
    /// - The longitude of the user
    /// - The distance in miles between the user and the attraction
    /// - The reward points associated with the attraction
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <returns>An array of NearbyAttractionInfos.</returns>
    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttractionInfos>> GetNearbyAttractions([FromQuery] string userName)
    {
        // Get the user's current location
        VisitedLocation visitedLocation = _tourGuideService.GetUserLocation(GetUser(userName));

        // Get the array of 5 maximum nearby attractions order by proximity
        Attraction[] attractions = _tourGuideService.GetNearbyAttractions(visitedLocation);

        // Create an array to store the NearbyAttractionInfos
        NearbyAttractionInfos[] nearbyAttractions = new NearbyAttractionInfos[5];
        int index = 0;

        // Populate the NearbyAttractionInfos array
        foreach (Attraction attraction in attractions)
        {
            nearbyAttractions[index++] = new NearbyAttractionInfos(
                attraction.AttractionName,
                attraction.Latitude,
                attraction.Longitude,
                visitedLocation.Location.Latitude,
                visitedLocation.Location.Longitude,
                0.621371 * _rewardsService.GetDistance(attraction, visitedLocation.Location),
                _rewardCentral.GetAttractionRewardPoints(attraction.AttractionId, GetUser(userName).UserId));
        }

        // Return the NearbyAttractionInfos array
        return Ok(nearbyAttractions);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}