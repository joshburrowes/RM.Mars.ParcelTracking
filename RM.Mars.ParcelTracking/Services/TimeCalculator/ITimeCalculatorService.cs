namespace RM.Mars.ParcelTracking.Services.TimeCalculator;

/// <summary>
/// Provides methods for calculating launch dates, estimated time of arrival (ETA), and estimated arrival dates for different parcel delivery services.
/// </summary>
public interface ITimeCalculatorService
{
    /// <summary>
    /// Gets the next launch date for the specified delivery service.
    /// For Standard, returns the next configured launch date (from settings), or the next cycle if the date has passed.
    /// For Express, returns the first Wednesday of the current or next month.
    /// Throws <see cref="ArgumentException"/> for unknown services.
    /// </summary>
    /// <param name="deliveryService">The delivery service name (e.g., "Standard", "Express").</param>
    /// <returns>The next launch <see cref="DateTime"/> for the service.</returns>
    DateTime GetLaunchDate(string deliveryService);

    /// <summary>
    /// Gets the estimated number of days for delivery for the specified service.
    /// Standard returns 180, Express returns 90. Throws <see cref="ArgumentException"/> for unknown services.
    /// </summary>
    /// <param name="deliveryService">The delivery service name.</param>
    /// <returns>Estimated delivery time in days.</returns>
    int GetEtaDays(string deliveryService);

    /// <summary>
    /// Calculates the estimated arrival date by adding ETA days to the launch date.
    /// </summary>
    /// <param name="launchDate">The launch <see cref="DateTime"/>.</param>
    /// <param name="etaDays">The estimated delivery time in days.</param>
    /// <returns>The estimated arrival <see cref="DateTime"/>.</returns>
    DateTime CalculateEstimatedArrivalDate(DateTime launchDate, int etaDays);
}