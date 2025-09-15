using RM.Mars.ParcelTracking.Models.Parcel;

namespace RM.Mars.ParcelTracking.Models.Requests;

public record CreateParcelRequest : BaseParcel
{
    public string DeliveryService { get; set; } = string.Empty;
}