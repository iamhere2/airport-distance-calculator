using System;
using AirportDistanceCalculator.Domain.Values;

namespace AirportDistanceCalculator.Application
{
    /// <summary>Interface of airport location service</summary>
    public interface IAirportLocator
    {
        /// <summary>Returns the location of an airport, specified by intl. code</summary>
        /// <exception cref="ArgumentOutOfRangeException">If the airport code is unknown or unsupported</exception>
        Location GetLocation(AirportCode airportCode);
    }
}