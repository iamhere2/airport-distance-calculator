using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AirportDistanceCalculator.Domain.Values
{
    /// <summary>Distance between two points</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Distance : IEquatable<Distance>
    {
        /// <summary>Equality threshold for values</summary>
        public const double EqualityThreshold = 0.01;

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

        /// <summary>Zero distance</summary>
        public static readonly Distance Zero = new Distance(0, DistanceUnit.Meters);

        /// <summary>Numerical value of distance</summary>
        public double Value { get; }

        /// <summary>Units of distance</summary>
        public DistanceUnit Units { get; }

        /// <summary>Returns <c>true</c>, if value is equal to <see cref="Zero"/></summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThreshold"/>
        /// </remarks>
        public bool IsZero() => Equals(Zero);

        /// <summary>Returns <c>true</c>, if other Distance equals to this</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThreshold"/>
        /// </remarks>
        public bool Equals(Distance other)
            => (Value, Units) == (other.Value, other.Units)
               || (Value == 0.0 && other.Value == 0.0)
               || Math.Abs(other.Convert(Units).Value - Value) < EqualityThreshold;

        /// <summary>Converts distance vaue to another units</summary>
        public Distance Convert(DistanceUnit targetUnits)
            => targetUnits == Units
                ? this
                : new Distance(Value * ConvertFactors[(Units, targetUnits)], targetUnits);

        private static readonly IDictionary<(DistanceUnit, DistanceUnit), double> ConvertFactors =
            new Dictionary<(DistanceUnit, DistanceUnit), double>
            {
                // TODO: Do something with low precision of conversions

                [(DistanceUnit.Meters, DistanceUnit.Meters)] = 1.0,
                [(DistanceUnit.Meters, DistanceUnit.Kilometers)] = 0.001,
                [(DistanceUnit.Meters, DistanceUnit.Miles)] = 1.0 / 1609.34,

                [(DistanceUnit.Kilometers, DistanceUnit.Kilometers)] = 1.0,
                [(DistanceUnit.Kilometers, DistanceUnit.Meters)] = 1000.0,
                [(DistanceUnit.Kilometers, DistanceUnit.Miles)] = 1.0 / 1609.34 / 1000.0,

                [(DistanceUnit.Miles, DistanceUnit.Miles)] = 1.0,
                [(DistanceUnit.Miles, DistanceUnit.Meters)] = 1609.34,
                [(DistanceUnit.Miles, DistanceUnit.Kilometers)] = 1609.34 / 1000.0
            };

        /// <inheritdoc/>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThreshold"/>
        /// </remarks>
        public override bool Equals(object? obj)
            => obj is Distance d && Equals(d);

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Value, Units).GetHashCode();

        /// <summary>Returns <c>true</c>, if other Distance equals to this.</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThreshold"/>
        /// </remarks>
        public static bool operator ==(Distance left, Distance right)
            => left.Equals(right);

        /// <summary>Returns <c>true</c>, if other Distance differs from this.</summary>
        /// <remarks>
        /// Comparison goes with threshold of <see cref="EqualityThreshold"/>
        /// </remarks>
        public static bool operator !=(Distance left, Distance right)
            => !(left == right);

        /// <inheritdoc/>
        public override string ToString() => $"{Value} {Units}";
    }
}
