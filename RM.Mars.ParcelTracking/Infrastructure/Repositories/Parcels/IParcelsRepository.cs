using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.Parcel;

namespace RM.Mars.ParcelTracking.Infrastructure.Repositories.Parcels;

/// <summary>
/// Interface for parcel data access and persistence operations.
/// </summary>
public interface IParcelsRepository
{
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