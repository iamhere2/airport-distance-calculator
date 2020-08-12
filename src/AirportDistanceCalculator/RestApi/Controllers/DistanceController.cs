using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.Domain.Values;
using AirportDistanceCalculator.RestApi.Results;
using Microsoft.AspNetCore.Mvc;

namespace AirportDistanceCalculator.RestApi.Controllers
{
    /// <summary>REST API controller for distance calculation</summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/airport-distance")]
    public class DistanceController : ControllerBase
    {
        private DistanceCalculator DistanceCalculator { get; }

        /// <summary>Constructor</summary>
        public DistanceController(DistanceCalculator distanceCalculator)
        {
            DistanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
        }

        /// <summary>Returns distance between airports in miles</summary>
        [HttpGet("miles")]
        public Task<AirportDistance> GetDistanceInMiles([Required] string from, [Required] string to)
            => GetDistanceAsync(from, to, DistanceUnit.Miles);

        /// <summary>Returns distance between airports in kilometers</summary>
        [HttpGet("kilometers")]
        public Task<AirportDistance> GetDistanceInKilometers([Required] string from, [Required] string to)
            => GetDistanceAsync(from, to, DistanceUnit.Kilometers);

        private async Task<AirportDistance> GetDistanceAsync(string from, string to, DistanceUnit unit)
        {
            var fromAirportCode = new AirportCode(from);
            var toAirportCode = new AirportCode(to);
            var distance = await DistanceCalculator.GetDistanceAsync(fromAirportCode, toAirportCode).ConfigureAwait(false);
            return new AirportDistance(fromAirportCode, toAirportCode, distance.Convert(unit));
        }
    }
}
