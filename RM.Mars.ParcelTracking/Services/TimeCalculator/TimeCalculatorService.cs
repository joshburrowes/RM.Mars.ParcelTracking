using Microsoft.Extensions.Options;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.TimeCalculator;

/// <summary>
/// Service for calculating launch dates, ETA days, and estimated arrival dates for parcel delivery services.
/// Reads the next standard launch date from configuration.
/// </summary>
public class TimeCalculatorService : ITimeCalculatorService
{
    private readonly IDateTimeProvider _timeProvider;
    private readonly DateTime _nextStandardLaunchDate;

    public TimeCalculatorService(IDateTimeProvider timeProvider, IOptions<TimeCalculatorOptions> options)
    {
        _timeProvider = timeProvider;
        string launchDateStr = options.Value.NextStandardLaunchDate;
        _nextStandardLaunchDate = DateTime.Parse(launchDateStr);
    }

    /// <inheritdoc/>
    public DateTime GetLaunchDate(string deliveryService)
    {
        DateTime currentUtcDate = _timeProvider.UtcNow;
        if (deliveryService == nameof(DeliveryServiceEnum.Standard))
        {
            DateTime nextStandardLaunch = _nextStandardLaunchDate;
            if (nextStandardLaunch < currentUtcDate)
            {
                nextStandardLaunch = nextStandardLaunch.AddMonths(26);
            }
            return nextStandardLaunch;
        }
        if (deliveryService != nameof(DeliveryServiceEnum.Express))
        {
            throw new ArgumentException("Invalid delivery service");
        }
        DateTime firstDayOfMonth = new(currentUtcDate.Year, currentUtcDate.Month, 1);
        int daysOffset = ((int)DayOfWeek.Wednesday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
        DateTime firstWednesday = firstDayOfMonth.AddDays(daysOffset);
        if (firstWednesday >= currentUtcDate)
        {
            return firstWednesday;
        }
        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
        daysOffset = ((int)DayOfWeek.Wednesday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
        firstWednesday = firstDayOfMonth.AddDays(daysOffset);
        return firstWednesday;
    }

    /// <inheritdoc/>
    public int GetEtaDays(string deliveryService)
    {
        return deliveryService switch
        {
            nameof(DeliveryServiceEnum.Standard) => 180,
            nameof(DeliveryServiceEnum.Express) => 90,
            _ => throw new ArgumentException("Invalid delivery service")
        };
    }

    /// <inheritdoc/>
    public DateTime CalculateEstimatedArrivalDate(DateTime launchDate, int etaDays) => launchDate.AddDays(etaDays);
}