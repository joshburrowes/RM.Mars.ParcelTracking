namespace RM.Mars.ParcelTracking.Utils.DateTimeProvider;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}