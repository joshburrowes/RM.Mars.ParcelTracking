using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Repositories.Parcels;
using RM.Mars.ParcelTracking.Services.TimeCalculator;

namespace RM.Mars.ParcelTracking.Services.Parcels;

/// <summary>
/// Service for core parcel lifecycle handling and business logic.
/// </summary>
public class ParcelService(
    IParcelsRepository parcelsRepository,
    ITimeCalculatorService timeCalculatorService,
    ILogger<ParcelService> logger)
    : IParcelService
{

    /// <inheritdoc/>
    public async Task<ParcelCreatedResponse?> ProcessParcelRequestAsync(CreateParcelRequest requestParcel)
    {
        ParcelDto? existingParcel = await parcelsRepository.GetParcelByBarcodeAsync(requestParcel.Barcode).ConfigureAwait(false);

        if (existingParcel != null)
        {
            return null;
        }

        DateTime launchDate = timeCalculatorService.GetLaunchDate(requestParcel.DeliveryService).Date;
        int etaDays = timeCalculatorService.GetEtaDays(requestParcel.DeliveryService);
        DateTime estimatedArrivalDate = timeCalculatorService.CalculateEstimatedArrivalDate(launchDate, etaDays).Date;

        ParcelCreatedResponse newParcel = new()
        {
            Barcode = requestParcel.Barcode,
            Sender = requestParcel.Sender,
            Recipient = requestParcel.Recipient,
            Contents = requestParcel.Contents,
            Status = ParcelStatus.Created,
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
    public async Task<bool> UpdateParcelStatus(ParcelDto parcel, ParcelStatus newStatus)
    {
        try
        {
            // Ensure date-only persists if consumer mutated times
            parcel.LaunchDate = parcel.LaunchDate.Date;
            parcel.EstimatedArrivalDate = parcel.EstimatedArrivalDate.Date;
            await parcelsRepository.UpdateParcelAsync(parcel, newStatus);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception whilst attempting to update status for parcel with barcode: {ParcelBarcode}", parcel.Barcode);
            return false;
        }

        return true;
    }
}