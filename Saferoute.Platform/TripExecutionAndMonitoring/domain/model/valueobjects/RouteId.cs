namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

/// <summary>
/// Value object referencing the route a trip executes.
/// <para>
/// The route is owned by the Fleet bounded context; Trip references it by identity through this
/// local value object, keeping the two contexts decoupled.
/// </para>
/// </summary>
public record RouteId
{
    /// <summary>
    /// The route identifier value.
    /// </summary>
    public long? Value { get; init; }

    /// <summary>
    /// Constructor enforcing the route identifier invariant.
    /// </summary>
    /// <param name="value">The route identifier</param>
    /// <exception cref="ArgumentException">If value is null or less than 1</exception>
    public RouteId(long? value)
    {
        if (value is null || value < 1)
        {
            throw new ArgumentException("RouteId must be a positive value");
        }
        Value = value;
    }
}