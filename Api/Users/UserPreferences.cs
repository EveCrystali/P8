namespace TourGuide.Users;

public class UserPreferences
{
    public int AttractionProximity { get; set; } = int.MaxValue;
    public int TripDuration { get; set; } = 1;
    public int TicketQuantity { get; set; } = 1;
    public int NumberOfAdults { get; set; } = 1;
    public int NumberOfChildren { get; set; } = 0;
}