using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.CommonUtils.Http;
using AirportDistanceCalculator.Domain.Values;
using AirportDistanceCalculator.Domain.Values.Exceptions;
using Polly;
using Polly.Caching;
using Polly.Registry;
using Serilog;

namespace AirportDistanceCalculator.RemoteServices
{
    /// <summary>Proxy for remote "places" service</summary>
    public class PlacesService : IAirportLocator
    {
        private static readonly ILogger Logger = Log.ForContext<PlacesService>();

        /// <summary>Constructor</summary>
        public PlacesService(HttpClient httpClient, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            CachePolicy = (policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry)))
                .TryGet<AsyncCachePolicy<JsonDocument>>("DefaultJsonCache", out var cachePolicy)
                    ? cachePolicy
                    : null;

            if (CachePolicy is null)
            {
                Logger.Warning("Cache policy not found, caching is disabled");
            }
        }

        private AsyncCachePolicy<JsonDocument>? CachePolicy { get; }

        private HttpClient HttpClient { get; }

        /// <inheritdoc/>
        Task<Location> IAirportLocator.GetLocationAsync(AirportCode airportCode)
            => GetLocationCachedAsync(airportCode);

        private async Task<Location> GetLocationCachedAsync(AirportCode airportCode)
        {
            var ctx = new Context($"{nameof(GetLocationCachedAsync)}:{airportCode.Code}");

            var getAirportTask = CachePolicy is null
                ? GetAirportAsync(airportCode)
                : CachePolicy.ExecuteAsync(ctx => GetAirportAsync(airportCode), ctx);

            return ParseLocation(await getAirportTask);
        }

        private async Task<JsonDocument> GetAirportAsync(AirportCode airportCode)
        {
            using var response = await HttpClient.GetAsync($"/airports/{airportCode.Code}");

            if (response.IsSuccessStatusCode)
            {
                return await response.GetJson();
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new AirportCodeException(airportCode.Code);
                }
                else
                {
                    throw new ApplicationException(
                        $"Error response from \"places\" service: {response.StatusCode}");
                }
            }
        }

        private Location ParseLocation(JsonDocument json)
        {
            var loc = json.RootElement.GetProperty("location");
            return new Location(loc.GetProperty("lat").GetDouble(), loc.GetProperty("lon").GetDouble());
        }
    }
}
