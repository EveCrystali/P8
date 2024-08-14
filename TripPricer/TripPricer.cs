using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPricer.Helpers;

namespace TripPricer;

public class TripPricer
{
    /// <summary>
    /// Calculates the price for a given trip.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="attractionId">The ID of the attraction.</param>
    /// <param name="adults">The number of adults.</param>
    /// <param name="children">The number of children.</param>
    /// <param name="nightsStay">The number of nights stay.</param>
    /// <param name="rewardsPoints">The rewards points.</param>
    /// <returns>A list of providers with their prices.</returns>
    public List<Provider> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
    {
        List<Provider> providers = [];
        HashSet<string> providersUsed = [];

        // HACK: certainly supposed to be optimized next to improve performance 
        // NOTE: Need to monitor the performance of this function to understand why it is slow when the number of providers is high
        // Sleep to simulate some latency
        Thread.Sleep(ThreadLocalRandom.Current.Next(1, 50));

        // Calculate the price for each provider
        for (int i = 0; i < 5; i++)
        {
            // Generate a random multiple for the price calculation
            int multiple = ThreadLocalRandom.Current.Next(100, 700);

            // Calculate the discount for children based on the number of children
            double childrenDiscount = (double)children / 3.0;

            // Calculate the price based on the number of adults, number of nights stay, multiple, and rewards points
            double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;

            // Ensure the price is not negative
            if (price < 0.0)
            {
                price = 0.0;
            }

            // Generate a unique provider name
            string provider;
            do
            {
                provider = GetProviderName(apiKey, adults);
            } while (providersUsed.Contains(provider));

            // Add the provider to the list of used providers
            providersUsed.Add(provider);

            // Create a new provider object with the attraction ID, provider name, and price
            Provider newProvider = new(attractionId, provider, price);

            // Add the new provider to the list of providers
            providers.Add(newProvider);
        }

        // Return the list of providers with their prices
        return providers;
    }

    /// <summary>
    /// Gets a provider name based on the given API key and the number of adults.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="adults">The number of adults.</param>
    /// <returns>A provider name.</returns>
    public static string GetProviderName(string apiKey, int adults)
    {
        // Generate a random multiple between 1 and 10 to select a provider name
        int multiple = ThreadLocalRandom.Current.Next(1, 10);

        // Switch on the multiple to select the provider name
        return multiple switch
        {
            1 => "Holiday Travels",
            2 => "Enterprize Ventures Limited",
            3 => "Sunny Days",
            4 => "FlyAway Trips",
            5 => "United Partners Vacations",
            6 => "Dream Trips",
            7 => "Live Free",
            8 => "Dancing Waves Cruselines and Partners",
            9 => "AdventureCo",
            // Default to Cure-Your-Blues
            _ => "Cure-Your-Blues",
        };        
    }
}
