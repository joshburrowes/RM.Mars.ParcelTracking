using FluentAssertions;
using NSubstitute;
using RM.Mars.ParcelTracking.Models.AuditTrail;
using RM.Mars.ParcelTracking.Services.AuditTrail;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;
using RM.Mars.ParcelTracking.Enums;

namespace RM.Mars.ParcelTracking.Test.Unit.Services
{
    [TestFixture]
    public class AuditTrailServiceTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private AuditTrailService _service;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _service = new AuditTrailService(_dateTimeProvider);
        }

        [Test]
        public void UpdateStatusHistory_AddsToNullHistory()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 10, 1));

            // Act
            List<StatusAuditTrail> result = _service.UpdateStatusHistory(null, ParcelStatus.Created);

            // Assert
            result.Should().HaveCount(1);
            result[0].Status.Should().Be(ParcelStatus.Created);
            result[0].TimeStamp.Should().Be("2025-10-01");
        }

        [Test]
        public void UpdateStatusHistory_AddsToExistingHistory()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 10, 2));
            List<StatusAuditTrail> history = new List<StatusAuditTrail>
            {
                new StatusAuditTrail { Status = ParcelStatus.Created, TimeStamp = "2025-10-01" }
            };

            // Act
            List<StatusAuditTrail> result = _service.UpdateStatusHistory(history, ParcelStatus.OnRocketToMars);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Status == ParcelStatus.Created && x.TimeStamp == "2025-10-01");
            result.Should().Contain(x => x.Status == ParcelStatus.OnRocketToMars && x.TimeStamp == "2025-10-02");
            result.Should().BeInAscendingOrder(x => x.TimeStamp);
        }

        [Test]
        public void UpdateStatusHistory_OrdersByTimeStamp()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 10, 3));
            List<StatusAuditTrail> history = new List<StatusAuditTrail>
            {
                new StatusAuditTrail { Status = ParcelStatus.OnRocketToMars, TimeStamp = "2025-10-02" },
                new StatusAuditTrail { Status = ParcelStatus.Created, TimeStamp = "2025-10-01" }
            };

            // Act
            List<StatusAuditTrail> result = _service.UpdateStatusHistory(history, ParcelStatus.LandedOnMars);

            // Assert
            result.Should().BeInAscendingOrder(x => x.TimeStamp);
            result[0].Status.Should().Be(ParcelStatus.Created);
            result[1].Status.Should().Be(ParcelStatus.OnRocketToMars);
            result[2].Status.Should().Be(ParcelStatus.LandedOnMars);
        }
    }
}
