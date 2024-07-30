using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location;

public class Attraction : Locations
{
    public string AttractionName { get; }
    public string City { get; }
    public string State { get; }
    public Guid AttractionId { get; }

    public Attraction(string attractionName, string city, string state, double latitude, double longitude) : base(latitude, longitude)
    {
        AttractionName = attractionName;
        City = city;
        State = state;
        AttractionId = Guid.NewGuid();
    }
}
