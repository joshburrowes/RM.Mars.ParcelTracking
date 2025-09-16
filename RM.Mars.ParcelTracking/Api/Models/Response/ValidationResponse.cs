using RM.Mars.ParcelTracking.Api.Models.Requests;

namespace RM.Mars.ParcelTracking.Api.Models.Response;

public record ValidationResponse
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public CreateParcelRequest? CreateParcelRequest { get; set; }
}