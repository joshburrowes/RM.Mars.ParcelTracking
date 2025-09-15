using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.StatusValidator;

/// <summary>
/// Service for validating parcel status transitions based on business rules and timing constraints.
/// </summary>
public class StatusValidation : IStatusValidation
{
    private readonly IDateTimeProvider dateTimeProvider;

    public StatusValidation(IDateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc/>
    public StatusValidationResponse ValidateStatus(ParcelDto parcel, string newStatus)
    {
        if (parcel.Status == ParcelStatus.Created.ToString())
        {
            if (newStatus == ParcelStatus.OnRocketToMars.ToString() && parcel.LaunchDate <= dateTimeProvider.UtcNow)
            {
                return new StatusValidationResponse { Valid = true };
            }

            if (parcel.LaunchDate > dateTimeProvider.UtcNow)
            {
                return new StatusValidationResponse
                {
                    Valid = false,
                    Reason =
                        $"Invalid status transition: Cannot update parcel status from: '{parcel.Status}' to: '{newStatus}' as Launch Date: '{parcel.LaunchDate}' is in the future."
                };
            }

            return DefaultInvalidTransition(parcel.Status, newStatus);
        }
        if (parcel.Status == ParcelStatus.OnRocketToMars.ToString())
        {
            if (newStatus == ParcelStatus.LandedOnMars.ToString() &&
                parcel.EstimatedArrivalDate <= dateTimeProvider.UtcNow)
            {
                return new StatusValidationResponse { Valid = true };
            }

            if (parcel.EstimatedArrivalDate > dateTimeProvider.UtcNow)
            {
                return new StatusValidationResponse
                {
                    Valid = false,
                    Reason =
                        "Invalid status transition: Estimated arrival date is in the future parcel hasn't landed yet."
                };
            }

            return DefaultInvalidTransition(parcel.Status, newStatus);

        }
        if (parcel.Status == ParcelStatus.LandedOnMars.ToString())
        {
            return newStatus == ParcelStatus.OutForMartianDelivery.ToString()
                ? new StatusValidationResponse { Valid = true }
                : DefaultInvalidTransition(parcel.Status, newStatus);
        }

        if (parcel.Status == ParcelStatus.OutForMartianDelivery.ToString())
        {
            if (newStatus == ParcelStatus.Delivered.ToString() || newStatus == ParcelStatus.Lost.ToString())
            {
                return new StatusValidationResponse { Valid = true };
            }

            return DefaultInvalidTransition(parcel.Status, newStatus);
        }

        if (parcel.Status == ParcelStatus.Lost.ToString())
        {
            return new StatusValidationResponse { Valid = false, Reason = "Invalid status transition: parcel is lost." };
        }

        if (parcel.Status == ParcelStatus.Delivered.ToString())
        {
            return new StatusValidationResponse
                { Valid = false, Reason = "Invalid status transition: parcel is already delivered." };
        }

        return new StatusValidationResponse { Valid = false, Reason = "Invalid status transition: unable to validate status." };
    }

    private static StatusValidationResponse DefaultInvalidTransition(string currentStatus, string newStatus)
    {
        return new StatusValidationResponse
        {
            Valid = false,
            Reason = $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newStatus}'"
        };
    }
}