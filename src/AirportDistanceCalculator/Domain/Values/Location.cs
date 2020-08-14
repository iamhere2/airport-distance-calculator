using System;
using System.Diagnostics;

namespace CTeleport.AirportDistanceCalculator.Domain.Values
{
    /// <summary>Geographical location of some point</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Location : IEquatable<Location>
    {
        /// <summary>Constructor</summary>
        public Location(double latitude, double longitude)
        {
            if (latitude < -90.0 || latitude > 90.0)
            {
                throw new ArgumentOutOfRangeException(nameof(latitude), latitude,
                    "Latitude must be in range [-90.0 .. 90.0]");
            }

            if (longitude < -180.0 || longitude > 180.0)
            {
                throw new ArgumentOutOfRangeException(nameof(longitude), longitude,
                    "Longitude must be in range [-180.0 .. 180.0]");
            }

            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>Latitude</summary>
        public double Latitude { get; }

        /// <summary>Longitude</summary>
        public double Longitude { get; }

        /// <summary>Returns <c>true</c>, if other Location equals to this.</summary>
        public bool Equals(Location other)
            => (Latitude, Longitude) == (other.Latitude, other.Longitude);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj is Location && Equals(obj);

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Latitude, Longitude).GetHashCode();

        /// <summary>Returns <c>true</c>, if other Location equals to this.</summary>
        public static bool operator ==(Location left, Location right)
            => left.Equals(right);

        /// <summary>Returns <c>true</c>, if other Location differs to this.</summary>
        public static bool operator !=(Location left, Location right)
            => !(left == right);

        /// <inheritdoc/>
        public override string ToString() => $"(Lt: {Latitude}; Lg: {Longitude})";

        /// <summary>The same as <see cref="Distance.Between(Location, Location)"/></summary>
        public static Distance operator -(Location a, Location b)
            => Distance.Between(a, b);
    }
}
