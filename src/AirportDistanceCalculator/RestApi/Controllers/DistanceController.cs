using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CTeleport.AirportDistanceCalculator.Application;
using CTeleport.AirportDistanceCalculator.Domain.Values;
using CTeleport.AirportDistanceCalculator.RestApi.Results;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CTeleport.AirportDistanceCalculator.RestApi.Controllers
{
    /// <summary>REST API controller for distance calculation</summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/airport-distance")]
    public class DistanceController : ControllerBase
    {
        private static readonly ILogger Logger = Log.ForContext<DistanceController>();

        private DistanceCalculator DistanceCalculator { get; }

        /// <summary>Constructor</summary>
        public DistanceController(DistanceCalculator distanceCalculator)
        {
            DistanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
        }

        /// <summary>Returns distance between airports in miles</summary>
        [HttpGet("miles")]
        // TODO: Make bindings for AirportCode type from query parameters instead of receiving just strings
        public Task<ActionResult<AirportDistance>> GetDistanceInMiles([Required] string from, [Required] string to)
            => GetDistanceAsync(from, to, DistanceUnit.Miles);

        /// <summary>Returns distance between airports in kilometers</summary>
        [HttpGet("kilometers")]
        public Task<ActionResult<AirportDistance>> GetDistanceInKilometers([Required] string from, [Required] string to)
            => GetDistanceAsync(from, to, DistanceUnit.Kilometers);

        private async Task<ActionResult<AirportDistance>> GetDistanceAsync(string from, string to, DistanceUnit unit)
        {
            try
            {
                var fromAirportCode = new AirportCode(from);
                var toAirportCode = new AirportCode(to);
                var distance = await DistanceCalculator.GetDistanceAsync(fromAirportCode, toAirportCode);
                return new AirportDistance(fromAirportCode, toAirportCode, distance.Convert(unit));
            }
            catch (AirportCodeException e)
            {
                Logger.Error(e, "Error while calculating airport distance {From} - {To}", from, to);
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
