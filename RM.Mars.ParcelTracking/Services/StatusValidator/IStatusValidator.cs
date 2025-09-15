using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.StatusValidator;

public interface IStatusValidator
{
    StatusValidation ValidateStatus(ParcelDto parcel, string newStatus);
}