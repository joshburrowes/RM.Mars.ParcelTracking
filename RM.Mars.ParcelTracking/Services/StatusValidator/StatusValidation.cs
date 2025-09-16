using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.StatusValidator;

/// <summary>
/// Service for validating parcel status transitions based on business rules and timing constraints.
/// </summary>
public class StatusValidation(IDateTimeProvider dateTimeProvider) : IStatusValidation
{
    /// <inheritdoc/>
    public StatusValidationResponse ValidateStatus(ParcelDto parcel, string newStatus)
    {
        if (!Enum.TryParse(newStatus, true, out ParcelStatus newParcelStatus))
        {
            return new StatusValidationResponse
            {
                Valid = false,
                Reason = "Invalid status transition: newStatus is not a valid ParcelStatus."
            };
        }

        ParcelStatus currentStatus = parcel.Status;
        StatusValidationResponse Result(bool valid, string reason = "") => new() { Valid = valid, Reason = reason, NewParcelStatus = newParcelStatus };

        switch (currentStatus)
        {
            case ParcelStatus.Created:
                if (newParcelStatus == ParcelStatus.OnRocketToMars && parcel.LaunchDate <= dateTimeProvider.UtcNow)
                    return Result(true);
                if (parcel.LaunchDate > dateTimeProvider.UtcNow)
                    return Result(false, $"Invalid status transition: Cannot update parcel status from: '{currentStatus}' to: '{newParcelStatus}' as Launch Date: '{parcel.LaunchDate}' is in the future.");
                return Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            case ParcelStatus.OnRocketToMars:
                if (newParcelStatus == ParcelStatus.LandedOnMars && parcel.EstimatedArrivalDate <= dateTimeProvider.UtcNow)
                    return Result(true);
                if (parcel.EstimatedArrivalDate > dateTimeProvider.UtcNow)
                    return Result(false, "Invalid status transition: Estimated arrival date is in the future, parcel hasn't landed yet.");
                return Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            case ParcelStatus.LandedOnMars:
                return newParcelStatus == ParcelStatus.OutForMartianDelivery
                    ? Result(true)
                    : Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            case ParcelStatus.OutForMartianDelivery:
                return newParcelStatus is ParcelStatus.Delivered or ParcelStatus.Lost
                    ? Result(true)
                    : Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            case ParcelStatus.Lost:
                return Result(false, "Invalid status transition: parcel is lost.");
            case ParcelStatus.Delivered:
                return Result(false, "Invalid status transition: parcel is already delivered.");
            default:
                return Result(false, "Invalid status transition: unable to validate status.");
        }
    }
}