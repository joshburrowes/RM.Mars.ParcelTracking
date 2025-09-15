using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Services.Validation;

namespace RM.Mars.ParcelTracking.Models.Validation;

public record ValidationResponse
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public CreateParcelRequest? CreateParcelRequest { get; set; }
}