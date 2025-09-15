using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Models.Validation;

namespace RM.Mars.ParcelTracking.Services.Parcels;

/// <summary>
/// Interface for parcel lifecycle handling and business logic.
/// </summary>
public interface IParcelService
{
    /// <summary>
    /// Processes a parcel creation request.
    /// </summary>
    Task<ParcelCreatedResponse?> ProcessParcelRequestAsync(CreateParcelRequest requestParcel);
    /// <summary>
    /// Gets a parcel by its barcode.
    /// </summary>
    Task<ParcelDto?> GetParcelByBarcodeAsync(string barcode);
    /// <summary>
    /// Updates the status of a parcel.
    /// </summary>
    Task<bool> UpdateParcelStatus(ParcelDto parcel, string newStatus);
}