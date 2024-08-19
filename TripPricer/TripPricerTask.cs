namespace TripPricer;

public class TripPricerTask : Task<List<Provider>>
{
    private readonly Guid _attractionId;
    private readonly string _apiKey;
    private readonly int _adults;
    private readonly int _children;
    private readonly int _nightsStay;

    public TripPricerTask(string apiKey, Guid attractionId, int adults, int children, int nightsStay)
        : base(() => new TripPricer().GetPrice(apiKey, attractionId, adults, children, nightsStay, 5))
    {
        _apiKey = apiKey;
        _attractionId = attractionId;
        _adults = adults;
        _children = children;
        _nightsStay = nightsStay;
    }

    public async Task<List<Provider>> ExecuteAsync()
    {
        TripPricer tripPricer = new();
        return await Task.Run(() => tripPricer.GetPrice(_apiKey, _attractionId, _adults, _children, _nightsStay, 5));
    }
}