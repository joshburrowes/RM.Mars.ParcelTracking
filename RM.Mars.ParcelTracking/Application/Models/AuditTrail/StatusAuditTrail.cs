using RM.Mars.ParcelTracking.Application.Enums;

namespace RM.Mars.ParcelTracking.Application.Models.AuditTrail;

public record StatusAuditTrail
{
    public ParcelStatus Status { get; set; }

    public string TimeStamp { get; set; } = string.Empty;
}