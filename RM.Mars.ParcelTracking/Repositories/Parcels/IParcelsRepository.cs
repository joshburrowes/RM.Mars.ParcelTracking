using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Response;

namespace RM.Mars.ParcelTracking.Repositories.Parcels;

/// <summary>
/// Interface for parcel data access and persistence operations.
/// </summary>
public interface IParcelsRepository
{
    /// <summary>
    /// Gets all parcels.
    /// </summary>
    List<ParcelDto> GetParcels();
    /// <summary>
    /// Adds a new parcel.
    /// </summary>
    Task AddParcelAsync(ParcelCreatedResponse parcelResponse);
    /// <summary>
    /// Gets a parcel by its barcode.
    /// </summary>
    Task<ParcelDto?> GetParcelByBarcodeAsync(string barcode);
    /// <summary>
    /// Updates the status of a parcel.
    /// </summary>
    Task UpdateParcelAsync(ParcelDto parcel, ParcelStatus newStatus);
}