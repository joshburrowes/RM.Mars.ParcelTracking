using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.Validation;

public interface IParcelRequestValidation
{
    ValidationResponse Validate(CreateParcelRequest request);
}