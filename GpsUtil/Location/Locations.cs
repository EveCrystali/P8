using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location;

public class Locations(double latitude, double longitude)
{
    public double Longitude { get; } = longitude;
    public double Latitude { get; } = latitude;
}
