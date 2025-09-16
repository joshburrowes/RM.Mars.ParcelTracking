namespace RM.Mars.ParcelTracking.Api.Models.Requests;

public record UpdateParcelStatusRequest
{
    public string NewStatus { get; set; } = string.Empty;
}