using System;
using AirportDistanceCalculator.Domain.Values;

namespace AirportDistanceCalculator.Application
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
        /// <exception cref="ArgumentOutOfRangeException">If the airport code is unknown or unsupported</exception>
        public Distance GetDistance(AirportCode from, AirportCode to)
        {
            if (from == to)
            {
                return Distance.Zero;
            }

            var fromLoc = AirportLocator.GetLocation(from);
            var toLoc = AirportLocator.GetLocation(to);

            var distanceMeters = Geolocation.GeoCalculator.GetDistance(
                fromLoc.Latitude, fromLoc.Longitude,
                toLoc.Latitude, toLoc.Longitude,
                decimalPlaces: 10,
                Geolocation.DistanceUnit.Meters);

            return new Distance(distanceMeters, DistanceUnit.Meters);
        }
    }
}
