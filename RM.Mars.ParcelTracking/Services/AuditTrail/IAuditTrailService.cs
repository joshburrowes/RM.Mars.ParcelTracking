using RM.Mars.ParcelTracking.Models.AuditTrail;

namespace RM.Mars.ParcelTracking.Services.AuditTrail;

public interface IAuditTrailService
{
    List<StatusAuditTrail> UpdateStatusHistory(List<StatusAuditTrail>? statusHistory, string status);
}