using RM.Mars.ParcelTracking.Models.AuditTrail;
using System.Text.Json.Serialization;

namespace RM.Mars.ParcelTracking.Models.Parcel;

public record ParcelDto : BaseParcel
{
    [JsonPropertyOrder(2)]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyOrder(3)]
    public DateTime LaunchDate { get; set; }

    [JsonPropertyOrder(4)]
    public DateTime EstimatedArrivalDate { get; set; }

    [JsonPropertyOrder(5)]
    public string Origin { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public string Destination { get; set; } = string.Empty;

    [JsonPropertyOrder(11)]
    public DateTime LastUpdated { get; set; }

    [JsonPropertyOrder(10)]
    public List<StatusAuditTrail>? History { get; set; }
}