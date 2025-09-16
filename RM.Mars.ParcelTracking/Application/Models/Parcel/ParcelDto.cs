using System.Text.Json.Serialization;
using RM.Mars.ParcelTracking.Common.Utils.Json;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.AuditTrail;

namespace RM.Mars.ParcelTracking.Application.Models.Parcel;

public record ParcelDto : BaseParcel
{
    [JsonPropertyOrder(2)]
    public ParcelStatus Status { get; set; }
    
    [JsonPropertyOrder(3)]
    [JsonConverter(typeof(DateOnlyDateTimeConverter))]
    public DateTime LaunchDate { get; set; }

    [JsonPropertyOrder(4)]
    [JsonConverter(typeof(DateOnlyDateTimeConverter))]
    public DateTime EstimatedArrivalDate { get; set; }

    [JsonPropertyOrder(5)]
    public string Origin { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public string Destination { get; set; } = string.Empty;

    [JsonPropertyOrder(12)]
    public DateTime LastUpdated { get; set; }

    [JsonPropertyOrder(11)]
    public List<StatusAuditTrail>? History { get; set; }
}