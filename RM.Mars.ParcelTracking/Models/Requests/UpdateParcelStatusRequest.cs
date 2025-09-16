using RM.Mars.ParcelTracking.Enums;

namespace RM.Mars.ParcelTracking.Models.Requests;

public record UpdateParcelStatusRequest
{
    public string NewStatus { get; set; }
}