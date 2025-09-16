using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.Parcel;
using RM.Mars.ParcelTracking.Application.Models.Validation;
using RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Application.Services.StatusValidator;

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
            {
                if (newParcelStatus == ParcelStatus.OnRocketToMars && parcel.LaunchDate <= dateTimeProvider.UtcNow)
                {
                    return Result(true);
                }
                
                return Result(false,
                    parcel.LaunchDate > dateTimeProvider.UtcNow
                        ? $"Invalid status transition: Cannot update parcel status from: '{currentStatus}' as Launch Date: '{parcel.LaunchDate}' is in the future."
                        : $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            }
            case ParcelStatus.OnRocketToMars:
            {
                if (newParcelStatus == ParcelStatus.LandedOnMars &&
                    parcel.EstimatedArrivalDate <= dateTimeProvider.UtcNow || newParcelStatus == ParcelStatus.Lost)
                {
                    return Result(true);
                }

                return Result(false,
                    parcel.EstimatedArrivalDate > dateTimeProvider.UtcNow
                        ? "Invalid status transition: Estimated arrival date is in the future, parcel hasn't landed yet."
                        : $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            }
            case ParcelStatus.LandedOnMars:
            {
                return newParcelStatus == ParcelStatus.OutForMartianDelivery
                    ? Result(true)
                    : Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            }
            case ParcelStatus.OutForMartianDelivery:
            {
                return newParcelStatus is ParcelStatus.Delivered or ParcelStatus.Lost
                    ? Result(true)
                    : Result(false, $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newParcelStatus}'");
            }
            case ParcelStatus.Lost:
            {
                return Result(false, "Invalid status transition: parcel is lost.");
            }
            case ParcelStatus.Delivered:
            {
                return Result(false, "Invalid status transition: parcel is already delivered.");
            }
            default:
            {
                return Result(false, "Invalid status transition: unable to validate status.");
            }
        }
    }
}