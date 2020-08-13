using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.Domain.Values;
using Polly;
using Polly.Caching;
using Polly.Registry;

namespace AirportDistanceCalculator.RemoteServices
{
    /// <summary>
    /// Proxy for remote service https://places-dev.cteleport.com,
    /// which implements <see cref="IAirportLocator"/>
    /// </summary>
    public class PlacesService : IAirportLocator
    {
        /// <summary>Constructor</summary>
        public PlacesService(HttpClient httpClient, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            PolicyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        }

        private IReadOnlyPolicyRegistry<string> PolicyRegistry { get; }

        private HttpClient HttpClient { get; }

        /// <inheritdoc/>
        Task<Location> IAirportLocator.GetLocationAsync(AirportCode airportCode)
            => GetLocationCachedAsync(airportCode);

        private async Task<Location> GetLocationCachedAsync(AirportCode airportCode)
        {
            var cachePolicy = GetCachePolicyIfDefined();
            var ctx = new Context($"{nameof(GetLocationCachedAsync)}:{airportCode.Code}");

            var getAirportTask = cachePolicy is null
                ? CallGetAirport(airportCode)
                : cachePolicy.ExecuteAsync(ctx => CallGetAirport(airportCode), ctx);

            return await ProcessGetLocationResponse(await getAirportTask, airportCode);
        }

        private IAsyncPolicy<HttpResponseMessage>? GetCachePolicyIfDefined()
            => PolicyRegistry.TryGet<AsyncCachePolicy<HttpResponseMessage>>("DefaultCache", out var policy)
                ? policy : null;

        private Task<Location> ProcessGetLocationResponse(HttpResponseMessage response, AirportCode airportCode)
        {
            if (response.IsSuccessStatusCode)
            {
                return ParseLocation(response.Content);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ArgumentOutOfRangeException(
                        $"Airport code is unknown or unsupported: {airportCode}");
                }
                else
                {
                    throw new ApplicationException(
                        $"Wrong response from Places service: {response.StatusCode}");
                }
            }
        }

        private async Task<HttpResponseMessage> CallGetAirport(AirportCode airportCode)
            => await HttpClient.GetAsync($"/airports/{airportCode.Code}");

        private async Task<Location> ParseLocation(HttpContent content)
        {
            var stream = await content.ReadAsStreamAsync();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            var json = await JsonDocument.ParseAsync(stream);
            var loc = json.RootElement.GetProperty("location");
            return new Location(loc.GetProperty("lat").GetDouble(), loc.GetProperty("lon").GetDouble());
        }
    }
}
