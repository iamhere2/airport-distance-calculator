using System;
using System.Net.Http;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.Domain.Values;
using AirportDistanceCalculator.RemoteServices;
using Xunit;

namespace IntegrationTests
{
    public class PlacesServiceTests
    {
        [Theory]
        [MemberData(nameof(SomeAirportLocations))]
        public async void Location_of_some_airports_can_be_determined(string code, Location expectedLocation)
        {
            // Arrange

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://places-dev.cteleport.com")
            };

            IAirportLocator locator = new PlacesService(httpClient);
            var airportCode = new AirportCode(code);

            // Act

            var location = await locator.GetLocationAsync(airportCode).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedLocation, location);
        }

        public static object[] SomeAirportLocations =
            new object[]
            {
                new object[] { "LGA", new Location(40.774252, -73.871617) },
                new object[] { "AMS", new Location(52.309069, 4.763385) }
            };
    }
}
