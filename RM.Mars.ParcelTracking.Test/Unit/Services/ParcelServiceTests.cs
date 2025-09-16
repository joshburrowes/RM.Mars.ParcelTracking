using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Logging;
using RM.Mars.ParcelTracking.Application.Services.Parcels;
using RM.Mars.ParcelTracking.Application.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Infrastructure.Repositories.Parcels;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Models.Parcel;

namespace RM.Mars.ParcelTracking.Test.Unit.Services
{
    [TestFixture]
    public class ParcelServiceTests
    {
        private IParcelsRepository _repo = null!;
        private ITimeCalculatorService _timeCalculatorService = null!;
        private ILogger<ParcelService> _logger = null!;
        private ParcelService _service = null!;

        private const string Barcode = "RMARS1234567890123456789M";
        private static readonly DateTime LaunchDate = new(2030, 01, 15);
        private const int EtaDays = 42;
        private static readonly DateTime EstimatedArrivalDate = LaunchDate.AddDays(EtaDays);

        [SetUp]
        public void Setup()
        {
            _repo = Substitute.For<IParcelsRepository>();
            _timeCalculatorService = Substitute.For<ITimeCalculatorService>();
            _logger = Substitute.For<ILogger<ParcelService>>();
            _service = new ParcelService(_repo, _timeCalculatorService, _logger);
        }

        [Test]
        public async Task ProcessParcelRequestAsync_ReturnsNull_WhenParcelExists_DoesNotAdd()
        {
            // Arrange
            CreateParcelRequest request = new() { Barcode = Barcode };
            _repo.GetParcelByBarcodeAsync(Barcode).Returns(new ParcelDto { Barcode = Barcode });

            // Act
            ParcelCreatedResponse? result = await _service.ProcessParcelRequestAsync(request);

            // Assert
            result.Should().BeNull();
            await _repo.DidNotReceive().AddParcelAsync(Arg.Any<ParcelCreatedResponse>());
        }

        [Test]
        public async Task ProcessParcelRequestAsync_CreatesParcel_WhenNotExists_SetsExpectedValues()
        {
            // Arrange
            CreateParcelRequest request = new()
            {
                Barcode = Barcode,
                Sender = "S",
                Recipient = "R",
                DeliveryService = nameof(DeliveryServiceEnum.Express),
                Contents = "Book"
            };
            _repo.GetParcelByBarcodeAsync(Barcode).Returns((ParcelDto?)null);
            _timeCalculatorService.GetLaunchDate(request.DeliveryService).Returns(LaunchDate);
            _timeCalculatorService.GetEtaDays(request.DeliveryService).Returns(EtaDays);
            _timeCalculatorService.CalculateEstimatedArrivalDate(LaunchDate, EtaDays).Returns(EstimatedArrivalDate);

            // Act
            ParcelCreatedResponse? result = await _service.ProcessParcelRequestAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Barcode.Should().Be(Barcode);
            result.Status.Should().Be(ParcelStatus.Created);
            result.LaunchDate.Should().Be(LaunchDate);
            result.EtaDays.Should().Be(EtaDays);
            result.EstimatedArrivalDate.Should().Be(EstimatedArrivalDate);
            result.Sender.Should().Be("S");
            result.Recipient.Should().Be("R");
            result.Contents.Should().Be("Book");

            await _repo.Received(1).AddParcelAsync(Arg.Is<ParcelCreatedResponse>(p =>
                p.Barcode == Barcode &&
                p.Status == ParcelStatus.Created &&
                p.LaunchDate == LaunchDate &&
                p.EtaDays == EtaDays &&
                p.EstimatedArrivalDate == EstimatedArrivalDate));
        }

        [Test]
        public async Task GetParcelByBarcodeAsync_ReturnsParcel_WhenExists()
        {
            // Arrange
            ParcelDto parcel = new() { Barcode = Barcode };
            _repo.GetParcelByBarcodeAsync(Barcode).Returns(parcel);

            // Act
            ParcelDto? result = await _service.GetParcelByBarcodeAsync(Barcode);

            // Assert
            result.Should().BeEquivalentTo(parcel);
        }

        [Test]
        public async Task GetParcelByBarcodeAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _repo.GetParcelByBarcodeAsync(Barcode).Returns((ParcelDto?)null);

            // Act
            ParcelDto? result = await _service.GetParcelByBarcodeAsync(Barcode);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateParcelStatus_ReturnsTrue_WhenUpdateSucceeds()
        {
            // Arrange
            ParcelDto parcel = new() { Barcode = Barcode, Status = ParcelStatus.Created };
            _repo.UpdateParcelAsync(parcel, ParcelStatus.OnRocketToMars).Returns(Task.CompletedTask);

            // Act
            bool result = await _service.UpdateParcelStatus(parcel, ParcelStatus.OnRocketToMars);

            // Assert
            result.Should().BeTrue();
            await _repo.Received(1).UpdateParcelAsync(parcel, ParcelStatus.OnRocketToMars);
        }

        [Test]
        public async Task UpdateParcelStatus_ReturnsFalse_WhenUpdateThrows()
        {
            // Arrange
            ParcelDto parcel = new() { Barcode = Barcode, Status = ParcelStatus.Created };
            _repo.UpdateParcelAsync(parcel, ParcelStatus.OnRocketToMars).Returns(_ => throw new Exception("fail"));

            // Act
            bool result = await _service.UpdateParcelStatus(parcel, ParcelStatus.OnRocketToMars);

            // Assert
            result.Should().BeFalse();
            await _repo.Received(1).UpdateParcelAsync(parcel, ParcelStatus.OnRocketToMars);
        }
    }
}
