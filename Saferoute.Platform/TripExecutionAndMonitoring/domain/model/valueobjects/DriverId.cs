namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

/// <summary>
/// Value object referencing the driver operating a trip.
/// <para>
/// The driver concept is owned by the Stakeholder bounded context; Trip references it by
/// identity through this local value object.
/// </para>
/// </summary>
/// <param name="Value">The driver identifier; must not be null or less than 1</param>
/// 
public record DriverId
{
    public long? Value { get; init; }
    
    public DriverId(long? value)
    {
        if (value is null || value < 1)
        {
            throw new ArgumentException("DriverId must be a positive value");
        }
        Value = value;
    }
}