using System.Text.Json.Serialization;
using RM.Mars.ParcelTracking.Common.Utils.Json;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.Parcel;

namespace RM.Mars.ParcelTracking.Api.Models.Response;

public record ParcelCreatedResponse : BaseParcel
{
    [JsonPropertyOrder(2)]
    public ParcelStatus Status { get; set; }

    [JsonPropertyOrder(7)]
    public string Destination { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public string Origin { get; set; } = string.Empty;

    [JsonPropertyOrder(5)]
    [JsonConverter(typeof(DateOnlyDateTimeConverter))]
    public DateTime EstimatedArrivalDate { get; set; }

    [JsonPropertyOrder(3)]
    [JsonConverter(typeof(DateOnlyDateTimeConverter))]
    public DateTime LaunchDate { get; set; }

    [JsonPropertyOrder(4)]
    public int EtaDays { get; set; }
}