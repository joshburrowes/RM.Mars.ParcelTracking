using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Repositories.Parcels;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.Parcels;

/// <summary>
/// Service for core parcel lifecycle handling and business logic.
/// </summary>
public class ParcelService : IParcelService
{
    private readonly IParcelsRepository parcelsRepository;
    private readonly ITimeCalculatorService timeCalculatorService;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IStatusValidation statusValidation;
    private readonly ILogger<ParcelService> logger;

    public ParcelService(
        IParcelsRepository parcelsRepository,
        ITimeCalculatorService timeCalculatorService,
        IDateTimeProvider dateTimeProvider,
        IStatusValidation statusValidation,
        ILogger<ParcelService> logger)
    {
        this.parcelsRepository = parcelsRepository;
        this.timeCalculatorService = timeCalculatorService;
        this.dateTimeProvider = dateTimeProvider;
        this.statusValidation = statusValidation;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ParcelCreatedResponse?> ProcessParcelRequestAsync(CreateParcelRequest requestParcel)
    {
        ParcelDto? existingParcel = await parcelsRepository.GetParcelByBarcodeAsync(requestParcel.Barcode).ConfigureAwait(false);

        if (existingParcel != null)
        {
            return null;
        }

        DateTime launchDate = timeCalculatorService.GetLaunchDate(requestParcel.DeliveryService);
        int etaDays = timeCalculatorService.GetEtaDays(requestParcel.DeliveryService);
        DateTime estimatedArrivalDate = timeCalculatorService.CalculateEstimatedArrivalDate(launchDate, etaDays);

        ParcelCreatedResponse newParcel = new()
        {
            Barcode = requestParcel.Barcode,
            Sender = requestParcel.Sender,
            Recipient = requestParcel.Recipient,
            Contents = requestParcel.Contents,
            Status = ParcelStatus.Created.ToString(),
            Origin = "Starport Thames Estuary",
            Destination = "New London",
            LaunchDate = launchDate,
            EstimatedArrivalDate = estimatedArrivalDate,
            EtaDays = etaDays,
        };

        await parcelsRepository.AddParcelAsync(newParcel);
        return newParcel;
    }

    /// <inheritdoc/>
    public async Task<ParcelDto?> GetParcelByBarcodeAsync(string barcode)
    {
        ParcelDto? parcel = await parcelsRepository.GetParcelByBarcodeAsync(barcode).ConfigureAwait(false);

        return parcel;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateParcelStatus(ParcelDto parcel, string newStatus)
    {
        try
        {
            await parcelsRepository.UpdateParcelAsync(parcel, newStatus);
        }
        catch (Exception e)
        {
            logger.LogError("Exception whilst attempting to update status for parcel with barcode: {ParcelBarcode} - {Error}", parcel.Barcode, e);
            return false;
        }

        return true;
    }
}