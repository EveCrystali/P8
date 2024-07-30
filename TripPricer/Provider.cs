using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPricer;

public class Provider
{
    public readonly string Name;
    public readonly double Price;
    public readonly Guid TripId;

    public Provider(Guid tripId, string name, double price)
    {
        this.Name = name;
        this.TripId = tripId;
        this.Price = price;
    }
}
