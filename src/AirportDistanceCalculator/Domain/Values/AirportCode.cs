using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AirportDistanceCalculator.Domain.Values
{
    /// <summary>International airport code</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct AirportCode : IEquatable<AirportCode>
    {
        private static readonly Regex CodeRegex = new Regex("^[A-Z]{3}$", RegexOptions.Compiled);

        /// <summary>Constructor</summary>
        public AirportCode(string code)
        {
            if (!CodeRegex.IsMatch(code))
            {
                throw new ArgumentOutOfRangeException(nameof(code), code,
                    "Airport code can have only three upper-case latin letters");
            }

            Code = code;
        }

        /// <summary>International three-letter code</summary>
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
