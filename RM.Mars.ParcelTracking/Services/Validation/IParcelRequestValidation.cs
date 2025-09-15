using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.Validation;

/// <summary>
/// Interface for validating parcel creation requests.
/// </summary>
public interface IParcelRequestValidation
{
    /// <summary>
    /// Validates a parcel creation request.
    /// </summary>
    /// <param name="request">The parcel creation request.</param>
    /// <returns>Validation result.</returns>
    ValidationResponse Validate(CreateParcelRequest request);
}