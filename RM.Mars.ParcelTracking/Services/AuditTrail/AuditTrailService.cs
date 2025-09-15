using RM.Mars.ParcelTracking.Models.AuditTrail;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.AuditTrail;

/// <summary>
/// Service for updating and managing parcel history (audit trail).
/// </summary>
public class AuditTrailService : IAuditTrailService
{
    private readonly IDateTimeProvider dateTimeProvider;

    public AuditTrailService(IDateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc/>
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