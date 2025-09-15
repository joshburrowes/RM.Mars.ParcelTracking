namespace RM.Mars.ParcelTracking.Services.TimeCalculator;

public interface ITimeCalculatorService
{
    DateTime GetLaunchDate(string deliveryService);
    int GetEtaDays(string deliveryService);
    DateTime CalculateEstimatedArrivalDate(DateTime launchDate, int etaDays);
}