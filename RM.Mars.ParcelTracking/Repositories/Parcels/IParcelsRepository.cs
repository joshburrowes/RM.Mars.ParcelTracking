using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Response;

namespace RM.Mars.ParcelTracking.Repositories.Parcels;

public interface IParcelsRepository
{
    List<ParcelDto> GetParcels();
    Task AddParcelAsync(ParcelCreatedResponse parcel);
    ParcelDto? GetParcelByBarcode(string barcode);
    Task UpdateParcelAsync(ParcelDto parcel);
}