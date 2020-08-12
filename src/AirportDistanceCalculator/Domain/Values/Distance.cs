using System;
using System.Diagnostics;

namespace AirportDistanceCalculator.Domain.Values
{
    /// <summary>Distance between two points</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Distance : IEquatable<Distance>
    {
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

        /// Returns <c>true</c>, if other Distance equals to this, both in value and units.
        public bool Equals(Distance other)
            => (Value, Units) == (other.Value, other.Units);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj is Distance d && Equals(d);

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Value, Units).GetHashCode();

        /// Returns <c>true</c>, if other Distance equals to this.
        public static bool operator ==(Distance left, Distance right)
            => left.Equals(right);

        /// Returns <c>true</c>, if other Distance differs from this.
        public static bool operator !=(Distance left, Distance right)
            => !(left == right);

        /// <inheritdoc/>
        public override string ToString() => $"{Value} {Units}";
    }
}
