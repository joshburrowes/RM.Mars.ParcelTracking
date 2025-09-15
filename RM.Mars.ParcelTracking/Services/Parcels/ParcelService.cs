using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Repositories.Parcels;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.Parcels;

public class ParcelService(
    IParcelsRepository parcelsRepository,
    ITimeCalculatorService timeCalculatorService,
    IDateTimeProvider dateTimeProvider,
    IStatusValidator statusValidator) : IParcelService
{
    public async Task<ParcelCreatedResponse?> ProcessParcelRequest(CreateParcelRequest requestParcel)
    {
        ParcelDto? existingParcel = parcelsRepository.GetParcelByBarcode(requestParcel.Barcode);

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

    public async Task<ParcelDto?> GetParcelByBarcode(string barcode)
    {
        ParcelDto? parcel = await parcelsRepository.GetParcelByBarcode(barcode).ConfigureAwait(false);

        return parcel;
    }

    public StatusValidation UpdateParcelStatus(string barcode, string newStatus)
    {
        ParcelDto? parcel = parcelsRepository.GetParcelByBarcode(barcode);

        if (parcel == null)
        {
            return new StatusValidation { Valid = false, Reason = $"Parcel with barcode: '{barcode}' not found." };
        }

        StatusValidation statusValidationResponse = statusValidator.ValidateStatus(parcel, newStatus);

        if (!statusValidationResponse.Valid)
        {
            return statusValidationResponse;
        }

        parcel.Status = newStatus;
        parcel.LastUpdated = dateTimeProvider.UtcNow;

        parcelsRepository.UpdateParcelAsync(parcel);

        return statusValidationResponse;
    }
}