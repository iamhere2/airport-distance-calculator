using CTeleport.AirportDistanceCalculator.Domain.Values;

namespace CTeleport.AirportDistanceCalculator.RestApi.Results
{
    /// <summary>Distance between two airports</summary>
    public readonly struct AirportDistance
    {
        /// <summary>Constructor</summary>
        public AirportDistance(AirportCode from, AirportCode to, Distance distance)
        {
            From = from;
            To = to;
            Distance = distance;
        }

        /// <summary>First airport's code</summary>
        public AirportCode From { get; }

        /// <summary>Second airport's code</summary>
        public AirportCode To { get; }

        /// <summary>Distance</summary>
        public Distance Distance { get; }
    }
}
