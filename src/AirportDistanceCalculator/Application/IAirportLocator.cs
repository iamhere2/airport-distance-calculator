using System.Threading.Tasks;
using CTeleport.AirportDistanceCalculator.Domain.Values;

namespace CTeleport.AirportDistanceCalculator.Application
{
    /// <summary>Interface of airport location service</summary>
    public interface IAirportLocator
    {
        /// <summary>Returns the location of an airport, specified by intl. code</summary>
        /// <exception cref="AirportCodeException">If the airport code is unknown or unsupported</exception>
        Task<Location> GetLocationAsync(AirportCode airportCode);
    }
}
