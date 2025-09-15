namespace RM.Mars.ParcelTracking.Models.AuditTrail;

public record StatusAuditTrail
{
    public string Status { get; set; } = string.Empty;

    public string TimeStamp { get; set; } = string.Empty;
}