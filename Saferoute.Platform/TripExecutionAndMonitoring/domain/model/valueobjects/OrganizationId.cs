namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

/// <summary>
/// Value object referencing the organization a trip belongs to.
/// <para>
/// Modeled locally inside the Trip bounded context (rather than shared with IAM/Fleet) so the
/// context remains independent and free of cross-context model coupling.
/// </para>
/// </summary>
public record OrganizationId
{
    /// <summary>
    /// The organization identifier value.
    /// </summary>
    public long? Value { get; init; }

    /// <summary>
    /// Constructor enforcing the organization identifier invariant.
    /// </summary>
    /// <param name="value">The organization identifier</param>
    /// <throws cref="ArgumentException">If value is null or less than 1</throws>
    public OrganizationId(long? value)
    {
        if (value is null || value < 1)
        {
            throw new ArgumentException("OrganizationId must be a positive value");
        }
        Value = value;
    }
}