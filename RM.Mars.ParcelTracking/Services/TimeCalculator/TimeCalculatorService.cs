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
        if (deliveryService == DeliveryServiceEnum.Standard.ToString())
        {
            DateTime nextStandardLaunch = _nextStandardLaunchDate;
            if (nextStandardLaunch < currentUtcDate)
            {
                nextStandardLaunch = nextStandardLaunch.AddMonths(26);
            }
            return nextStandardLaunch;
        }
        if (deliveryService != DeliveryServiceEnum.Express.ToString())
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
        if(deliveryService == DeliveryServiceEnum.Standard.ToString())
        {
            return 180;
        }
        if (deliveryService == DeliveryServiceEnum.Express.ToString())
        {
            return 90;
        }
        throw new ArgumentException("Invalid delivery service");
    }

    /// <inheritdoc/>
    public DateTime CalculateEstimatedArrivalDate(DateTime launchDate, int etaDays) => launchDate.AddDays(etaDays);
}