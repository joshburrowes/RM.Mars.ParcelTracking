namespace RM.Mars.ParcelTracking.Utils.DateTimeProvider;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}