using System.Text.Json.Serialization;

namespace RM.Mars.ParcelTracking.Models.Parcel;

public record BaseParcel
{
    [JsonPropertyOrder(1)]
    public string Barcode { get; set; } = string.Empty;

    [JsonPropertyOrder(8)]
    public string Sender { get; set; } = string.Empty;

    [JsonPropertyOrder(9)]
    public string Recipient { get; set; } = string.Empty;

    [JsonPropertyOrder(10)]
    public string Contents { get; set; } = string.Empty;
}