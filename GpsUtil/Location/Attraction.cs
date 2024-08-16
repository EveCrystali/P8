using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location;

public class Attraction(string attractionName, string city, string state, double latitude, double longitude) : Locations(latitude, longitude)
{
    public string AttractionName { get; } = attractionName;
    public string City { get; } = city;
    public string State { get; } = state;
    public Guid AttractionId { get; } = Guid.NewGuid();
}
