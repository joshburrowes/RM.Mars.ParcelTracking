using RM.Mars.ParcelTracking.Enums;

namespace RM.Mars.ParcelTracking.Models.AuditTrail;

public record StatusAuditTrail
{
    public ParcelStatus Status { get; set; }

    public string TimeStamp { get; set; } = string.Empty;
}