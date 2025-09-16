using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;

namespace RM.Mars.ParcelTracking.Application.Services.Validation;

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