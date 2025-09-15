using RM.Mars.ParcelTracking.Models.AuditTrail;

namespace RM.Mars.ParcelTracking.Models.Parcel;

public record Parcel
{
    public string Barcode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public DateTime EstimatedArrivalDate { get; set; }

    public DateTime LaunchDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public int EtaDays { get; set; }

    public string Contents { get; set; } = string.Empty;

    public List<StatusAuditTrail>? History { get; set; }
}