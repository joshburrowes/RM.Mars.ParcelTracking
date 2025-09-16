using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentAssertions;
using RM.Mars.ParcelTracking.Api.Controllers;
using RM.Mars.ParcelTracking.Application.Services.Parcels;
using RM.Mars.ParcelTracking.Application.Services.StatusValidator;
using RM.Mars.ParcelTracking.Application.Services.Validation;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Models.Parcel;
using RM.Mars.ParcelTracking.Application.Models.Validation;

namespace RM.Mars.ParcelTracking.Test.Unit.Controllers
{
    [TestFixture]
    public class ParcelsControllerTests
    {
        private IParcelService _parcelService;
        private ILogger<ParcelsController> _logger;
        private IParcelRequestValidation _requestValidation;
        private IStatusValidation _statusValidation;
        private ParcelsController _controller;

        [SetUp]
        public void Setup()
        {
            _parcelService = Substitute.For<IParcelService>();
            _logger = Substitute.For<ILogger<ParcelsController>>();
            _requestValidation = Substitute.For<IParcelRequestValidation>();
            _statusValidation = Substitute.For<IStatusValidation>();

            _controller = new ParcelsController(
                _parcelService,
                _logger,
                _requestValidation,
                _statusValidation);
        }

        [Test]
        public async Task CreateParcel_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "", Sender = "", Recipient = "" };
            _requestValidation.Validate(request).Returns(new ValidationResponse { IsValid = false, ErrorMessage = "Invalid" });

            // Act
            IActionResult result = await _controller.CreateParcel(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid");
        }

        [Test]
        public async Task CreateParcel_ReturnsCreated_WhenParcelCreated()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", DeliveryService = "Express", Contents = "Book" };
            _requestValidation.Validate(request).Returns(new ValidationResponse { IsValid = true });
            _parcelService.ProcessParcelRequestAsync(request).Returns(new ParcelCreatedResponse { Barcode = "RMARS1234567890123456789M", Status = ParcelStatus.Created });

            // Act
            IActionResult result = await _controller.CreateParcel(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeOfType<ParcelCreatedResponse>()
                .And.Subject.As<ParcelCreatedResponse>().Barcode.Should().Be("RMARS1234567890123456789M");
        }

        [Test]
        public async Task GetParcel_ReturnsBadRequest_WhenBarcodeEmpty()
        {
            // Arrange + Act
            IActionResult result = await _controller.GetParcel("");
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Barcode is required.");
        }

        [Test]
        public async Task GetParcel_ReturnsNotFound_WhenParcelMissing()
        {
            // Arrange
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns((ParcelDto?)null);
            // Act
            IActionResult result = await _controller.GetParcel("RMARS1234567890123456789M");
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task GetParcel_ReturnsOk_WhenParcelExists()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", Status = ParcelStatus.Created };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns(parcel);
            // Act
            IActionResult result = await _controller.GetParcel("RMARS1234567890123456789M");
            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(parcel);
        }

        [Test]
        public async Task UpdateStatus_ReturnsBadRequest_WhenBarcodeEmpty()
        {
            // Arrange
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = nameof(ParcelStatus.OnRocketToMars) };
            // Act
            IActionResult result = await _controller.UpdateStatus("", request);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Barcode is required.");
        }

        [Test]
        public async Task UpdateStatus_ReturnsNotFound_WhenParcelMissing()
        {
            // Arrange
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = nameof(ParcelStatus.OnRocketToMars) };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns((ParcelDto?)null);
            // Act
            IActionResult result = await _controller.UpdateStatus("RMARS1234567890123456789M", request);
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task UpdateStatus_ReturnsBadRequest_WhenNewStatusEmpty()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", Status = ParcelStatus.Created };
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = "" };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns(parcel);
            // Act
            IActionResult result = await _controller.UpdateStatus("RMARS1234567890123456789M", request);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("newStatus must have a value.");
        }

        [Test]
        public async Task UpdateStatus_ReturnsBadRequest_WhenStatusValidationFails()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", Status = ParcelStatus.Created };
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = "InvalidStatus" };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns(parcel);
            _statusValidation.ValidateStatus(parcel, "InvalidStatus").Returns(new StatusValidationResponse { Valid = false, Reason = "Invalid" });
            // Act
            IActionResult result = await _controller.UpdateStatus("RMARS1234567890123456789M", request);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid");
        }

        [Test]
        public async Task UpdateStatus_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", Status = ParcelStatus.Created };
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = nameof(ParcelStatus.OnRocketToMars) };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns(parcel);
            _statusValidation.ValidateStatus(parcel, request.NewStatus).Returns(new StatusValidationResponse { Valid = true, NewParcelStatus = ParcelStatus.OnRocketToMars });
            _parcelService.UpdateParcelStatus(parcel, ParcelStatus.OnRocketToMars).Returns(true);
            // Act
            IActionResult result = await _controller.UpdateStatus("RMARS1234567890123456789M", request);
            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task UpdateStatus_ReturnsServerError_WhenUpdateFails()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Sender = "S", Recipient = "R", Status = ParcelStatus.Created };
            UpdateParcelStatusRequest request = new UpdateParcelStatusRequest { NewStatus = nameof(ParcelStatus.OnRocketToMars) };
            _parcelService.GetParcelByBarcodeAsync("RMARS1234567890123456789M").Returns(parcel);
            _statusValidation.ValidateStatus(parcel, request.NewStatus).Returns(new StatusValidationResponse { Valid = true, NewParcelStatus = ParcelStatus.OnRocketToMars });
            _parcelService.UpdateParcelStatus(parcel, ParcelStatus.OnRocketToMars).Returns(false);
            // Act
            IActionResult result = await _controller.UpdateStatus("RMARS1234567890123456789M", request);
            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }
    }
}
