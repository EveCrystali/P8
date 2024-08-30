using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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

        // Sleep to simulate some latency
        Thread.Sleep(ThreadLocalRandom.Current.Next(1, 50));

        // NOTE: i should not be bigger than the number of unique GetProviderName could generate
        // Otherwise the loop become infinite
        // Calculate the price for each provider

        int maximum = GetProviderNameCaseCount();

        for (int i = 0; i < maximum; i++)
        {
            // Generate a random multiple for the price calculation
            int multiple = ThreadLocalRandom.Current.Next(100, 700);

            // Calculate the discount for children based on the number of children
            double childrenDiscount = children / 3.0;

            // Calculate the price based on the number of adults, number of nights stay, multiple, and rewards points
            double price = (multiple * adults) + (multiple * childrenDiscount * nightsStay) + 0.99 - rewardsPoints;

            // Ensure the price is not negative
            if (price < 0.0)
            {
                price = 0.0;
            }

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
        // NextInt is inclusive of the minimum and exclusive of the maximum !!
        int index = ThreadLocalRandom.Current.Next(0, providerNames.Length);

        return providerNames[index];
    }

    /// <summary>
    /// Gets the number of unique provider names.
    /// </summary>
    /// <returns>The number of unique provider names.</returns>
    public static int GetProviderNameCaseCount()
    {
        return providerNames.Length;
    }

    private static readonly string[] providerNames =
    [
        "Holiday Travels",
        "Enterprize Ventures Limited",
        "Sunny Days",
        "FlyAway Trips",
        "United Partners Vacations",
        "Dream Trips",
        "Live Free",
        "Dancing Waves Cruselines and Partners",
        "AdventureCo",
        "Cure-Your-Blues"
     ];

}