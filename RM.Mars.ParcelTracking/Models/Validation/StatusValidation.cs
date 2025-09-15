namespace RM.Mars.ParcelTracking.Models.Validation;

public record StatusValidation
{
    public bool Valid { get; set; }

    public string Reason { get; set; } = string.Empty;
}