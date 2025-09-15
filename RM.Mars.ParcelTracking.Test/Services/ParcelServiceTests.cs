using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Logging;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Repositories.Parcels;
using RM.Mars.ParcelTracking.Services.Parcels;
using RM.Mars.ParcelTracking.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Test.Services
{
    [TestFixture]
    public class ParcelServiceTests
    {
        private IParcelsRepository _repo;
        private ITimeCalculatorService _timeCalculatorService;
        private IDateTimeProvider _dateTimeProvider;
        private IStatusValidation _statusValidation;
        private ILogger<ParcelService> _logger;
        private ParcelService _service;

        [SetUp]
        public void Setup()
        {
            _repo = Substitute.For<IParcelsRepository>();
            _timeCalculatorService = Substitute.For<ITimeCalculatorService>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _statusValidation = Substitute.For<IStatusValidation>();
            _logger = Substitute.For<ILogger<ParcelService>>();
            _service = new ParcelService(_repo, _timeCalculatorService, _dateTimeProvider, _statusValidation, _logger);
        }

        [Test]
        public async Task ProcessParcelRequestAsync_ReturnsNull_WhenParcelExists()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest { Barcode = "RMARS1234567890123456789M" };
            _repo.GetParcelByBarcodeAsync(request.Barcode).Returns(new ParcelDto { Barcode = request.Barcode });

            // Act
            ParcelCreatedResponse result = await _service.ProcessParcelRequestAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ProcessParcelRequestAsync_CreatesParcel_WhenNotExists()
        {
            // Arrange
            CreateParcelRequest request = new CreateParcelRequest
            {
                Barcode = "RMARS1234567890123456789M",
                Sender = "S",
                Recipient = "R",
                DeliveryService = "Express",
                Contents = "Book"
            };
            _repo.GetParcelByBarcodeAsync(request.Barcode).Returns((ParcelDto)null);
            _timeCalculatorService.GetLaunchDate(request.DeliveryService).Returns(DateTime.UtcNow.Date);
            _timeCalculatorService.GetEtaDays(request.DeliveryService).Returns(10);
            _timeCalculatorService.CalculateEstimatedArrivalDate(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(DateTime.UtcNow.Date.AddDays(10));

            // Act
            ParcelCreatedResponse result = await _service.ProcessParcelRequestAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Barcode.Should().Be(request.Barcode);
            result.Sender.Should().Be(request.Sender);
            result.Recipient.Should().Be(request.Recipient);
            result.Contents.Should().Be(request.Contents);
            result.Status.Should().Be("Created");
            result.LaunchDate.Should().Be(DateTime.UtcNow.Date);
            result.EtaDays.Should().Be(10);
            result.EstimatedArrivalDate.Should().Be(DateTime.UtcNow.Date.AddDays(10));
        }

        [Test]
        public async Task GetParcelByBarcodeAsync_ReturnsParcel_WhenExists()
        {
            // Arrange
            string barcode = "RMARS1234567890123456789M";
            ParcelDto parcel = new ParcelDto { Barcode = barcode };
            _repo.GetParcelByBarcodeAsync(barcode).Returns(parcel);

            // Act
            ParcelDto result = await _service.GetParcelByBarcodeAsync(barcode);

            // Assert
            result.Should().BeEquivalentTo(parcel);
        }

        [Test]
        public async Task GetParcelByBarcodeAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            string barcode = "RMARS1234567890123456789M";
            _repo.GetParcelByBarcodeAsync(barcode).Returns((ParcelDto)null);

            // Act
            ParcelDto result = await _service.GetParcelByBarcodeAsync(barcode);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateParcelStatus_ReturnsTrue_WhenUpdateSucceeds()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Status = "Created" };
            string newStatus = "InTransit";
            _repo.UpdateParcelAsync(parcel, newStatus).Returns(Task.CompletedTask);

            // Act
            bool result = await _service.UpdateParcelStatus(parcel, newStatus);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task UpdateParcelStatus_ReturnsFalse_WhenUpdateThrows()
        {
            // Arrange
            ParcelDto parcel = new ParcelDto { Barcode = "RMARS1234567890123456789M", Status = "Created" };
            string newStatus = "InTransit";
            _repo.UpdateParcelAsync(parcel, newStatus).Returns(x => { throw new Exception("fail"); });

            // Act
            bool result = await _service.UpdateParcelStatus(parcel, newStatus);

            // Assert
            result.Should().BeFalse();
        }
    }
}
