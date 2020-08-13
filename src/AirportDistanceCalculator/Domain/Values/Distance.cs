using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AirportDistanceCalculator.Domain.Values
{
    /// <summary>Distance between two points</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Distance : IEquatable<Distance>
    {
        /// <summary>Equality threshold for distance values, meters</summary>
        /// <remarks>
        /// Distance, less than that, considered to be zero.
        /// </remarks>
        public const double EqualityThresholdMeters = 1.0;

        /// <summary>Zero distance</summary>
        public static readonly Distance Zero = new Distance(0, DistanceUnit.Meters);

        /// <summary>Constructor</summary>
        public Distance(double value, DistanceUnit units)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Distance can't be less than 0.0");
            }

            Value = value;
            Units = units;
        }

        /// <summary>Numerical value of distance</summary>
        public double Value { get; }

        /// <summary>Units of distance</summary>
        public DistanceUnit Units { get; }

        /// <summary>Returns <c>true</c>, if value is equal to <see cref="Zero"/></summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThresholdMeters"/>
        /// </remarks>
        public bool IsZero() => Equals(Zero);

        /// <summary>Returns <c>true</c>, if other Distance equals to this</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThresholdMeters"/>
        /// </remarks>
        public bool Equals(Distance other)
            => (Value, Units) == (other.Value, other.Units)
               || (Value == 0.0 && other.Value == 0.0)
               || Math.Abs(other.Convert(DistanceUnit.Meters).Value
                               - Convert(DistanceUnit.Meters).Value)
                    < EqualityThresholdMeters;

        /// <summary>Converts distance vaue to another units</summary>
        public Distance Convert(DistanceUnit targetUnits)
            => (targetUnits == Units)
                ? this
                : new Distance(Value * ConvertFactors[(Units, targetUnits)], targetUnits);

        private static readonly IDictionary<(DistanceUnit from, DistanceUnit to), double> ConvertFactors =
            new ReadOnlyDictionary<(DistanceUnit from, DistanceUnit to), double>(
                new Dictionary<(DistanceUnit from, DistanceUnit to), double>
                {
                    [(DistanceUnit.Meters, DistanceUnit.Meters)] = 1.0,
                    [(DistanceUnit.Meters, DistanceUnit.Kilometers)] = 1.0 / 1000.0,
                    [(DistanceUnit.Meters, DistanceUnit.Miles)] = 1.0 / 1609.34,

                    [(DistanceUnit.Kilometers, DistanceUnit.Kilometers)] = 1.0,
                    [(DistanceUnit.Kilometers, DistanceUnit.Meters)] = 1000.0,
                    [(DistanceUnit.Kilometers, DistanceUnit.Miles)] = 1.0 / 1609.34 / 1000.0,

                    [(DistanceUnit.Miles, DistanceUnit.Miles)] = 1.0,
                    [(DistanceUnit.Miles, DistanceUnit.Meters)] = 1609.34,
                    [(DistanceUnit.Miles, DistanceUnit.Kilometers)] = 1609.34 / 1000.0
                });

        /// <inheritdoc/>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThresholdMeters"/>
        /// </remarks>
        public override bool Equals(object? obj)
            => obj is Distance d && Equals(d);

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Value, Units).GetHashCode();

        /// <summary>Returns <c>true</c>, if other Distance equals to this.</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThresholdMeters"/>
        /// </remarks>
        public static bool operator ==(Distance left, Distance right)
            => left.Equals(right);

        /// <summary>Returns <c>true</c>, if other Distance differs from this.</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThresholdMeters"/>
        /// </remarks>
        public static bool operator !=(Distance left, Distance right)
            => !(left == right);

        /// <inheritdoc/>
        public override string ToString() => $"{Value} {Units}";

        /// <summary>Calculates the distance (in meters) between two locations.</summary>
        public static Distance Between(Location a, Location b)
        {
            var distanceMeters = Geolocation.GeoCalculator.GetDistance(
                a.Latitude, a.Longitude,
                b.Latitude, b.Longitude,
                decimalPlaces: 10,
                Geolocation.DistanceUnit.Meters);

            return new Distance(distanceMeters, DistanceUnit.Meters);
        }
    }
}
