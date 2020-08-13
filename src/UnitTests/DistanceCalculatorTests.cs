using System;
using System.Threading.Tasks;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.Domain.Values;
using Moq;
using Xunit;

namespace UnitTests
{
    public class DistanceCalculatorTests
    {
        [Fact]
        public async void The_same_airport_is_at_zero_distance_without_external_calls()
        {
            // Assert

            var locatorMock = new Mock<IAirportLocator>(MockBehavior.Strict);
            locatorMock
                .Setup(l => l.GetLocationAsync(It.IsAny<AirportCode>()))
                .Throws(new Exception("There is no need to call locator for the same airports"));

            var calculator = new DistanceCalculator(locatorMock.Object);

            // Act

            var distance = await calculator.GetDistanceAsync(
                new AirportCode("ABC"), new AirportCode("ABC")).ConfigureAwait(false);

            // Assert

            Assert.True(distance.IsZero());
        }

        [Fact]
        public async void Calculates_distance_for_some_locations()
        {
            // Assert

            var locatorMock = new Mock<IAirportLocator>(MockBehavior.Strict);

            locatorMock
                .Setup(l => l.GetLocationAsync(new AirportCode("DME")))
                .Returns(Task.FromResult(new Location(55.414566, 37.899494)));

            locatorMock
                .Setup(l => l.GetLocationAsync(new AirportCode("JFK")))
                .Returns(Task.FromResult(new Location(40.642335, -73.78817)));

            var calculator = new DistanceCalculator(locatorMock.Object);

            // Act

            var distance = await calculator.GetDistanceAsync(
                new AirportCode("DME"), new AirportCode("JFK")).ConfigureAwait(false);

            // Assert

            Assert.Equal(DistanceUnit.Meters, distance.Units);
            Assert.Equal(7544373.408, distance.Value, 3);
            Assert.Equal(new Distance(4687.867950, DistanceUnit.Miles), distance);
        }
    }
}
