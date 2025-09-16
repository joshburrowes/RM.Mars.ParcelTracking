using FluentAssertions;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Services.Validation;

namespace RM.Mars.ParcelTracking.Test.Unit.Services
{
    [TestFixture]
    public class ParcelRequestValidationTests
    {
        private ParcelRequestValidation _service;

        [SetUp]
        public void Setup()
        {
            _service = new ParcelRequestValidation();
        }

        [Test]
        public void Validate_ReturnsInvalid_WhenBarcodeIsEmpty()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "", DeliveryService = nameof(DeliveryServiceEnum.Express) };

            // Act
            var result = _service.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Barcode is required");
        }

        [Test]
        public void Validate_ReturnsInvalid_WhenBarcodeIsWrongFormat()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "WRONGFORMAT", DeliveryService = nameof(DeliveryServiceEnum.Express) };

            // Act
            var result = _service.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("invalid format");
        }

        [Test]
        public void Validate_ReturnsInvalid_WhenDeliveryServiceIsWrong()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "RMARS1234567890123456789M", DeliveryService = "Invalid" };

            // Act
            var result = _service.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("must be either");
        }

        [Test]
        public void Validate_ReturnsValid_WhenAllFieldsCorrect()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "RMARS1234567890123456789M", DeliveryService = nameof(DeliveryServiceEnum.Express) };

            // Act
            var result = _service.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }
    }
}
