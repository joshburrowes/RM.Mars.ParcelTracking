using RM.Mars.ParcelTracking.Application.Enums;

namespace RM.Mars.ParcelTracking.Application.Models.Validation;

public record StatusValidationResponse
{
    public bool Valid { get; set; }

    public string Reason { get; set; } = string.Empty;

    public ParcelStatus NewParcelStatus { get; set; }
}