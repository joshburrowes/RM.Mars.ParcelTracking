namespace RM.Mars.ParcelTracking.Models.Parcel;

public record BaseParcel
{
    public string Barcode { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
}