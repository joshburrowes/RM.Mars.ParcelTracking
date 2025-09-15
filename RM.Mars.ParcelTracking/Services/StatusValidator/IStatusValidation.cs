using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.StatusValidator;

/// <summary>
/// Interface for validating parcel status transitions.
/// </summary>
public interface IStatusValidation
{
    /// <summary>
    /// Validates a status transition for a parcel.
    /// </summary>
    /// <param name="parcel">The parcel to validate.</param>
    /// <param name="newStatus">The new status to validate.</param>
    /// <returns>Validation result.</returns>
    StatusValidationResponse ValidateStatus(ParcelDto parcel, string newStatus);
}