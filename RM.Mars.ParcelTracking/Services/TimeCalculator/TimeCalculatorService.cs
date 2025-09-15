using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Services.TimeCalculator;

/// <summary>
/// Provides methods for calculating launch dates, estimated time of arrival (ETA),  and estimated arrival dates for
/// different delivery services.
/// </summary>
/// <remarks>This service supports calculations for delivery services such as Standard and Express. The
/// calculations are based on predefined rules for launch dates and ETAs.</remarks>
/// <param name="timeProvider"></param>
public class TimeCalculatorService(IDateTimeProvider timeProvider) : ITimeCalculatorService
{
    public DateTime GetLaunchDate(string deliveryService)
    {
        DateTime currentUtcDate = timeProvider.UtcNow;
        if (deliveryService == DeliveryServiceEnum.Standard.ToString())
        {
            DateTime nextStandardLaunch = new(year: 2025, month:10, day: 1);

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

    public DateTime CalculateEstimatedArrivalDate(DateTime launchDate, int etaDays) => launchDate.AddDays(etaDays);
}