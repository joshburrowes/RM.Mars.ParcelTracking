namespace RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;

/// <summary>
/// Abstraction for providing current UTC date and time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}