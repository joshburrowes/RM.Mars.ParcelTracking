namespace RM.Mars.ParcelTracking.Application.Services.TimeCalculator;

/// <summary>
/// Options for configuring TimeCalculatorService.
/// </summary>
public class TimeCalculatorOptions
{
    /// <summary>
    /// The next standard launch date for parcels (ISO format: yyyy-MM-dd).
    /// </summary>
    public string NextStandardLaunchDate { get; set; } = "2025-10-01";
}
