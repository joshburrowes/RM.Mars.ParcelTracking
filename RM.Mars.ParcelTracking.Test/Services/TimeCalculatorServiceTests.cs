using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Options;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Test.Services
{
    [TestFixture]
    public class TimeCalculatorServiceTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private IOptions<TimeCalculatorOptions> _options;
        private TimeCalculatorService _service;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var options = Options.Create(new TimeCalculatorOptions { NextStandardLaunchDate = "2025-10-01" });
            _options = options;
            _service = new TimeCalculatorService(_dateTimeProvider, _options);
        }

        [Test]
        public void GetLaunchDate_ReturnsNextStandardLaunch_ForStandard()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 9, 1));

            // Act
            DateTime launchDate = _service.GetLaunchDate(DeliveryServiceEnum.Standard.ToString());

            // Assert
            launchDate.Should().Be(new DateTime(2025, 10, 1));
        }

        [Test]
        public void GetLaunchDate_ReturnsFutureStandardLaunch_IfPast()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 1, 1));

            // Act
            DateTime launchDate = _service.GetLaunchDate(DeliveryServiceEnum.Standard.ToString());

            // Assert
            launchDate.Month.Should().Be(12); // 26 months after Oct 2025 is Dec 2027
            launchDate.Year.Should().Be(2027);
        }

        [Test]
        public void GetLaunchDate_ReturnsFirstWednesday_ForExpress()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 10, 1)); // Wednesday

            // Act
            DateTime launchDate = _service.GetLaunchDate(DeliveryServiceEnum.Express.ToString());

            // Assert
            launchDate.Should().Be(new DateTime(2025, 10, 1));
        }

        [Test]
        public void GetLaunchDate_Throws_ForInvalidService()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            Action act = () => _service.GetLaunchDate("Invalid");

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid delivery service");
        }

        [Test]
        public void GetEtaDays_Returns180_ForStandard()
        {
            // Act
            int eta = _service.GetEtaDays(DeliveryServiceEnum.Standard.ToString());
            // Assert
            eta.Should().Be(180);
        }

        [Test]
        public void GetEtaDays_Returns90_ForExpress()
        {
            // Act
            int eta = _service.GetEtaDays(DeliveryServiceEnum.Express.ToString());
            // Assert
            eta.Should().Be(90);
        }

        [Test]
        public void GetEtaDays_Throws_ForInvalidService()
        {
            // Act
            Action act = () => _service.GetEtaDays("Invalid");
            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid delivery service");
        }

        [Test]
        public void CalculateEstimatedArrivalDate_AddsDays()
        {
            // Act
            DateTime result = _service.CalculateEstimatedArrivalDate(new DateTime(2025, 10, 1), 10);
            // Assert
            result.Should().Be(new DateTime(2025, 10, 11));
        }
    }
}
