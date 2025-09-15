using RM.Mars.ParcelTracking.Models.AuditTrail;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.AuditTrail;

public class AuditTrailService(IDateTimeProvider dateTimeProvider) : IAuditTrailService
{
    public List<StatusAuditTrail> UpdateStatusHistory(List<StatusAuditTrail>? statusHistory, string statusToAdd)
    {

        StatusAuditTrail auditTrailStatus = new()
        {
            Status = statusToAdd,
            TimeStamp = dateTimeProvider.UtcNow.ToString("yyyy-MM-dd")
        };

        if (statusHistory == null)
        {
            statusHistory =
            [
                auditTrailStatus
            ];

            return statusHistory;
        }

        statusHistory.Add(auditTrailStatus);
        statusHistory = statusHistory.OrderBy(x => x.TimeStamp).ToList();
        return statusHistory;
    }
}