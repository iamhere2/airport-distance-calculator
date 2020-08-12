using System.ComponentModel.DataAnnotations;
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
        /// <summary>Test method</summary>
        [HttpGet("test")]
        public string Test(string arg) => arg;

        /// <summary>Test method</summary>
        [HttpGet("miles")]
        public AirportDistance GetDistanceInMiles([Required] string from, [Required] string to)
            => new AirportDistance(
                new AirportCode(from),
                new AirportCode(to),
                new Distance(123.0, DistanceUnit.Miles));
    }
}
