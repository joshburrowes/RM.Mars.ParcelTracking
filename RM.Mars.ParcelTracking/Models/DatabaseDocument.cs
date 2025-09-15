using RM.Mars.ParcelTracking.Models.Parcel;

namespace RM.Mars.ParcelTracking.Models;

public class DatabaseDocument
{
    public List<ParcelDto> Parcels { get; set; } = [];
}