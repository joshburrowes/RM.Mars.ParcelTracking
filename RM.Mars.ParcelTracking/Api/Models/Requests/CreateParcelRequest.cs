using RM.Mars.ParcelTracking.Application.Models.Parcel;

namespace RM.Mars.ParcelTracking.Api.Models.Requests;

public record CreateParcelRequest : BaseParcel
{
    public string DeliveryService { get; set; } = string.Empty;
}