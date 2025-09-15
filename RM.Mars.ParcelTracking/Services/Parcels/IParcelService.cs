using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.Parcels;

public interface IParcelService
{
    Task<ParcelCreatedResponse?> ProcessParcelRequest(CreateParcelRequest requestParcel);
    Task<ParcelDto?> GetParcelByBarcode(string barcode);
    StatusValidation UpdateParcelStatus(string barcode, string newStatus);
}