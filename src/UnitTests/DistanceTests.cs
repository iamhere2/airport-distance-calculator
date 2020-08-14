using CTeleport.AirportDistanceCalculator.Domain.Values;
using Xunit;

namespace UnitTests
{
    public class DistanceTests
    {
        [Theory]
        [MemberData(nameof(EqualZeroValues))]
        public void Zero_distances_are_equal_to_each_other(Distance a, Distance b)
            => Assert.Equal(a, b);

        public static readonly object[] EqualZeroValues =
            new object[]
            {
                new object[] { Distance.Zero, Distance.Zero },
                new object[] { Distance.Zero, new Distance(0.0, DistanceUnit.Meters) },
                new object[] { Distance.Zero, new Distance(0.0, DistanceUnit.Miles) },
                new object[] { Distance.Zero, new Distance(0.0, DistanceUnit.Kilometers) },
            };

        [Theory]
        [MemberData(nameof(EqualNonZeroValues))]
        public void Non_zero_distances_are_equal_to_each_other(Distance a, Distance b)
            => Assert.Equal(a, b);

        public static readonly object[] EqualNonZeroValues =
            new object[]
            {
                new object[] { new Distance(100.0, DistanceUnit.Meters), new Distance(0.1, DistanceUnit.Kilometers) },
                new object[] { new Distance(100.0, DistanceUnit.Miles), new Distance(160934.0, DistanceUnit.Meters) },
                new object[] { new Distance(100.0, DistanceUnit.Kilometers), new Distance(62.1371, DistanceUnit.Miles) },
            };
    }
}
