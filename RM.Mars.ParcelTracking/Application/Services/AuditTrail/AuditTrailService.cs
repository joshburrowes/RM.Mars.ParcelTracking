using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.AuditTrail;
using RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Application.Services.AuditTrail;

/// <summary>
/// Service for updating and managing parcel history (audit trail).
/// </summary>
public class AuditTrailService(IDateTimeProvider dateTimeProvider) : IAuditTrailService
{
    /// <inheritdoc/>
    public List<StatusAuditTrail> UpdateStatusHistory(List<StatusAuditTrail>? statusHistory, ParcelStatus statusToAdd)
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