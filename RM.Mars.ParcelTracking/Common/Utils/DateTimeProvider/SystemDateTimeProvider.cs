namespace RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;

/// <summary>
/// Provides the current UTC date and time using the system clock.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;
}