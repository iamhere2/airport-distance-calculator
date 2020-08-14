using System.Runtime.Serialization;

namespace CTeleport.AirportDistanceCalculator.Domain.Values
{
    /// <summary>Enum of distance units</summary>
    public enum DistanceUnit
    {
        /// <summary>Meters</summary>
        [EnumMember(Value = "m")]
        Meters,

        /// <summary>Kilometers</summary>
        [EnumMember(Value = "Km")]
        Kilometers,

        /// <summary>Miles</summary>
        [EnumMember(Value = "Mi")]
        Miles
    }
}
