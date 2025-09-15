using FluentAssertions;
using NSubstitute;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Test.Services
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
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created.ToString(), LaunchDate = DateTime.UtcNow.AddDays(-1) };
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.OnRocketToMars.ToString());

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_CreatedToOnRocketToMars_InvalidIfLaunchDateFuture()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created.ToString(), LaunchDate = DateTime.UtcNow.AddDays(1) };
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.OnRocketToMars.ToString());

            // Assert
            result.Valid.Should().BeFalse();
            result.Reason.Should().Contain("Launch Date");
        }

        [Test]
        public void ValidateStatus_CreatedToOther_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Created.ToString(), LaunchDate = DateTime.UtcNow.AddDays(-1) };
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Delivered.ToString());

            // Assert
            result.Valid.Should().BeFalse();
            result.Reason.Should().Contain("cannot move");
        }

        [Test]
        public void ValidateStatus_OnRocketToMarsToLandedOnMars_ValidIfArrivalPassed()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OnRocketToMars.ToString(), EstimatedArrivalDate = DateTime.UtcNow.AddDays(-1) };
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.LandedOnMars.ToString());

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OnRocketToMarsToLandedOnMars_InvalidIfArrivalFuture()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OnRocketToMars.ToString(), EstimatedArrivalDate = DateTime.UtcNow.AddDays(1) };
            _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.LandedOnMars.ToString());

            // Assert
            result.Valid.Should().BeFalse();
            result.Reason.Should().Contain("future");
        }

        [Test]
        public void ValidateStatus_LandedOnMarsToOutForMartianDelivery_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.LandedOnMars.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.OutForMartianDelivery.ToString());

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_LandedOnMarsToOther_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.LandedOnMars.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Delivered.ToString());

            // Assert
            result.Valid.Should().BeFalse();
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToDelivered_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Delivered.ToString());

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToLost_Valid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Lost.ToString());

            // Assert
            result.Valid.Should().BeTrue();
        }

        [Test]
        public void ValidateStatus_OutForMartianDeliveryToOther_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.OutForMartianDelivery.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.OnRocketToMars.ToString());

            // Assert
            result.Valid.Should().BeFalse();
        }

        [Test]
        public void ValidateStatus_LostToAny_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Lost.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Created.ToString());

            // Assert
            result.Valid.Should().BeFalse();
            result.Reason.Should().Contain("lost");
        }

        [Test]
        public void ValidateStatus_DeliveredToAny_Invalid()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Status = ParcelStatus.Delivered.ToString() };

            // Act
            StatusValidationResponse result = _service.ValidateStatus(parcel, ParcelStatus.Created.ToString());

            // Assert
            result.Valid.Should().BeFalse();
            result.Reason.Should().Contain("delivered");
        }
    }
}
