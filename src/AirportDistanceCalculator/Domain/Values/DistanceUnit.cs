using System.Runtime.Serialization;

namespace AirportDistanceCalculator.Domain.Values
{
    /// <summary>Enum of distance units</summary>
    public enum DistanceUnit
    {
        /// <summary>Miles</summary>
        [EnumMember(Value = "MI")]
        Miles,

        /// <summary>Kilometers</summary>
        [EnumMember(Value = "KM")]
        Kilometers
    }
}
