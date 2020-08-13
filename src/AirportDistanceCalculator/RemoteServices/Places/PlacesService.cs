using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.Domain.Values;

namespace AirportDistanceCalculator.RemoteServices
{
    /// <summary>
    /// Proxy for remote service https://places-dev.cteleport.com,
    /// which implements <see cref="IAirportLocator"/>
    /// </summary>
    public class PlacesService : IAirportLocator
    {
        /// <summary>Constructor</summary>
        public PlacesService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        private HttpClient HttpClient { get; }

        /// <inheritdoc/>
        async Task<Location> IAirportLocator.GetLocationAsync(AirportCode airportCode)
        {
            var response = await HttpClient.GetAsync($"/airports/{airportCode.Code}");

            if (response.IsSuccessStatusCode)
            {
                return await ParseLocation(response.Content);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentOutOfRangeException($"Airport code is unknown or unsupported: {airportCode}");
                }
                else
                {
                    throw new ApplicationException($"Wrong response from Places service: {response.StatusCode}");
                }
            }
        }

        private async Task<Location> ParseLocation(HttpContent content)
        {
            using var stream = await content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(stream);
            var loc = json.RootElement.GetProperty("location");
            return new Location(loc.GetProperty("lat").GetDouble(), loc.GetProperty("lon").GetDouble());
        }
    }
}
