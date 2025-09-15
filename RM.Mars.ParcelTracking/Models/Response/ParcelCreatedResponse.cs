using RM.Mars.ParcelTracking.Models.Parcel;

namespace RM.Mars.ParcelTracking.Models.Response;

public record ParcelCreatedResponse : BaseParcel
{
    public string Status { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public DateTime EstimatedArrivalDate { get; set; }

    public DateTime LaunchDate { get; set; }

    public int EtaDays { get; set; }
}