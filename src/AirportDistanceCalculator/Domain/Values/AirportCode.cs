using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CTeleport.AirportDistanceCalculator.Domain.Values
{
    /// <summary>International airport code (IATA)</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct AirportCode : IEquatable<AirportCode>
    {
        // TODO: Consider using reference data file for the exhaustive list of IATA codes (pros, cons)

        private static readonly Regex CodeRegex = new Regex("^[A-Z]{3}$", RegexOptions.Compiled);

        /// <summary>Constructor</summary>
        /// <exception cref="AirportCodeException">If the airport code is unknown or unsupported</exception>
        public AirportCode(string code)
        {
            if (!CodeRegex.IsMatch(code))
            {
                throw new AirportCodeException(code);
            }

            Code = code;
        }

        /// <summary>International three-letter code (IATA)</summary>
        public string Code { get; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj is AirportCode ac && Equals(ac);

        /// <summary>Returns <c>true</c>, if other AirportCode equals to this.</summary>
        public bool Equals(AirportCode other) =>
            other.Code.Equals(Code, StringComparison.InvariantCulture);

        /// <inheritdoc/>
        public override int GetHashCode() => Code.GetHashCode();

        /// <summary>Returns <c>true</c>, if other AirportCode equals to this.</summary>
        public static bool operator ==(AirportCode left, AirportCode right)
            => left.Equals(right);

        /// <summary>Returns <c>true</c>, if other AirportCode differs from this.</summary>
        public static bool operator !=(AirportCode left, AirportCode right)
            => !(left == right);

        /// <inheritdoc/>
        public override string ToString() => Code;
    }
}
