using FluentAssertions;
using NSubstitute;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Test.Unit.Services
{
    [TestFixture]
    public class StatusValidationTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private StatusValidation _service;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _service = new StatusValidation(_dateTimeProvider);
        }

        [Test]
        public void ValidateStatus_CreatedToOnRocketToMars_ValidIfLaunchDatePassed()
        {
            // Arrange
            DateTime now = new DateTime(2030, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created, LaunchDate = now.AddDays(-1) };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.OnRocketToMars));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_CreatedToOnRocketToMars_InvalidIfLaunchDateFuture()
        {
            // Arrange
            DateTime now = new DateTime(2030, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            DateTime futureLaunch = now.AddDays(1);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created, LaunchDate = futureLaunch };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.OnRocketToMars));

            // Assert
            string expected = $"Invalid status transition: Cannot update parcel status from: '{ParcelStatus.Created}' to: '{ParcelStatus.OnRocketToMars}' as Launch Date: '{futureLaunch}' is in the future.";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_CreatedToOther_Invalid()
        {
            // Arrange
            DateTime now = new DateTime(2030, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created, LaunchDate = now.AddDays(-1) };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Delivered));

            // Assert
            string expected = $"Invalid status transition: Parcels cannot move from: '{ParcelStatus.Created}' to: '{ParcelStatus.Delivered}'";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_OnRocketToMarsToLandedOnMars_ValidIfArrivalPassed()
        {
            // Arrange
            DateTime now = new DateTime(2030, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OnRocketToMars, EstimatedArrivalDate = now.AddDays(-1) };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.LandedOnMars));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OnRocketToMarsToLost_Valid()
        {
            // Arrange
            DateTime now = new DateTime(2030, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OnRocketToMars, EstimatedArrivalDate = now.AddDays(-1) };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Lost));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OnRocketToMarsToLandedOnMars_InvalidIfArrivalFuture()
        {
            // Arrange
            DateTime now = new DateTime(2030, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow.Returns(now);
            DateTime futureEta = now.AddDays(1);
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OnRocketToMars, EstimatedArrivalDate = futureEta };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.LandedOnMars));

            // Assert
            string expected = "Invalid status transition: Estimated arrival date is in the future, parcel hasn't landed yet.";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_LandedOnMarsToOutForMartianDelivery_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.LandedOnMars };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.OutForMartianDelivery));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_LandedOnMarsToOther_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.LandedOnMars };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Delivered));

            // Assert
            string expected = $"Invalid status transition: Parcels cannot move from: '{ParcelStatus.LandedOnMars}' to: '{ParcelStatus.Delivered}'";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToDelivered_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Delivered));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToLost_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Lost));

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToOther_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.OnRocketToMars));

            // Assert
            string expected = $"Invalid status transition: Parcels cannot move from: '{ParcelStatus.OutForMartianDelivery}' to: '{ParcelStatus.OnRocketToMars}'";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_LostToAny_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Lost };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Created));

            // Assert
            string expected = "Invalid status transition: parcel is lost.";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }

        [Test]
        public void ValidateStatus_DeliveredToAny_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Delivered };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, nameof(ParcelStatus.Created));

            // Assert
            string expected = "Invalid status transition: parcel is already delivered.";
            result.Valid.Should().BeFalse();
            result.Reason.Should().Be(expected);
        }
    }
}
