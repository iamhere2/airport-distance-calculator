using System;
using System.Threading.Tasks;
using CTeleport.AirportDistanceCalculator.CommonUtils.Tasks;
using CTeleport.AirportDistanceCalculator.Domain.Values;

namespace CTeleport.AirportDistanceCalculator.Application
{
    /// <summary>Calculates distances</summary>
    public class DistanceCalculator
    {
        /// <summary>Constructor</summary>
        public DistanceCalculator(IAirportLocator airportLocator)
        {
            AirportLocator = airportLocator ?? throw new ArgumentNullException(nameof(airportLocator));
        }

        private IAirportLocator AirportLocator { get; }

        /// <summary>Returns the distance (in meters) between airports</summary>
        /// <exception cref="AirportCodeException">If the airport code is unknown or unsupported</exception>
        public async Task<Distance> GetDistanceAsync(AirportCode from, AirportCode to)
        {
            if (from == to)
            {
                return Distance.Zero;
            }

            var (fromLoc, toLoc) = await
                (
                    AirportLocator.GetLocationAsync(from),
                    AirportLocator.GetLocationAsync(to)
                )
                .WhenAll();

            return fromLoc - toLoc;
        }
    }
}
