using RM.Mars.ParcelTracking.Enums;

namespace RM.Mars.ParcelTracking.Models.Validation;

public record StatusValidationResponse
{
    public bool Valid { get; set; }

    public string Reason { get; set; } = string.Empty;

    public ParcelStatus NewParcelStatus { get; set; }
}