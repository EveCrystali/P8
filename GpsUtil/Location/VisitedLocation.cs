namespace GpsUtil.Location;

public class VisitedLocation(Guid userId, Locations location, DateTime timeVisited)
{
    public Guid UserId { get; } = userId;
    public Locations Location { get; } = location;
    public DateTime TimeVisited { get; } = timeVisited;
}
