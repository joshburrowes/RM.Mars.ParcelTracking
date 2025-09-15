using RM.Mars.ParcelTracking.Models.AuditTrail;

namespace RM.Mars.ParcelTracking.Services.AuditTrail;

/// <summary>
/// Interface for updating parcel status history (audit trail).
/// </summary>
public interface IAuditTrailService
{
    /// <summary>
    /// Updates the status history for a parcel.
    /// </summary>
    /// <param name="statusHistory">Existing status history.</param>
    /// <param name="status">New status to add.</param>
    /// <returns>Updated status history list.</returns>
    List<StatusAuditTrail> UpdateStatusHistory(List<StatusAuditTrail>? statusHistory, string status);
}