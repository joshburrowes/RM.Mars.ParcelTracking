using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.StatusValidator;

/// <summary>
/// Provides functionality to validate status transitions for parcels based on their current status, the desired new
/// status, and associated timing constraints.
/// </summary>
/// <remarks>This class enforces business rules for parcel status transitions, ensuring that transitions adhere to
/// predefined criteria such as launch dates, estimated arrival dates, and valid status sequences. For example, a parcel
/// cannot transition to "OnRocketToMars" if its launch date is in the future, or to "LandedOnMars" if its estimated
/// arrival date has not yet passed.</remarks>
/// <param name="dateTimeProvider"></param>
public class StatusValidator(IDateTimeProvider dateTimeProvider) : IStatusValidator
{
    public StatusValidation ValidateStatus(ParcelDto parcel, string newStatus)
    {
        if (parcel.Status == ParcelStatus.Created.ToString())
        {
            if (newStatus == ParcelStatus.OnRocketToMars.ToString() && parcel.LaunchDate <= dateTimeProvider.UtcNow)
            {
                return new StatusValidation { Valid = true };
            }

            if (parcel.LaunchDate > dateTimeProvider.UtcNow)
            {
                return new StatusValidation
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
                return new StatusValidation { Valid = true };
            }

            if (parcel.EstimatedArrivalDate > dateTimeProvider.UtcNow)
            {
                return new StatusValidation
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
                ? new StatusValidation { Valid = true }
                : DefaultInvalidTransition(parcel.Status, newStatus);
        }

        if (parcel.Status == ParcelStatus.OutForMartianDelivery.ToString())
        {
            if (newStatus == ParcelStatus.Delivered.ToString() || newStatus == ParcelStatus.Lost.ToString())
            {
                return new StatusValidation { Valid = true };
            }

            return DefaultInvalidTransition(parcel.Status, newStatus);
        }

        if (parcel.Status == ParcelStatus.Lost.ToString())
        {
            return new StatusValidation { Valid = false, Reason = "Invalid status transition: parcel is lost." };
        }

        if (parcel.Status == ParcelStatus.Delivered.ToString())
        {
            return new StatusValidation
                { Valid = false, Reason = "Invalid status transition: parcel is already delivered." };
        }

        return new StatusValidation { Valid = false, Reason = "Invalid status transition: unable to validate status." };
    }

    private static StatusValidation DefaultInvalidTransition(string currentStatus, string newStatus)
    {
        return new StatusValidation
        {
            Valid = false,
            Reason = $"Invalid status transition: Parcels cannot move from: '{currentStatus}' to: '{newStatus}'"
        };
    }
}