using Powertech.Platform.Trip.Domain.Model.ValueObjects;

namespace Powertech.Platform.Trip.Domain.Model.Entities;

/// <summary>
///     Entity that records an incident reported by the driver during a trip.
/// </summary>
/// <remarks>
///     <c>Incident</c> is a child entity of the <see cref="Aggregates.Trip" /> aggregate. It is
///     created through the aggregate root when an incident is reported.
/// </remarks>
public class Incident
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Incident()
    {
        Id = new IncidentId();
        Description = new IncidentDescription();
    }

    /// <summary>
    ///     Creates a new incident with the given description, stamping the report time.
    /// </summary>
    /// <param name="description">The validated incident description.</param>
    public Incident(IncidentDescription description)
    {
        Id = IncidentId.New();
        Description = description;
        ReportedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Local identity of the incident within the aggregate.</summary>
    public IncidentId Id { get; private set; }

    /// <summary>The description of the incident.</summary>
    public IncidentDescription Description { get; private set; }

    /// <summary>The moment the incident was reported.</summary>
    public DateTimeOffset ReportedAt { get; private set; }

    /// <summary>Returns the incident description text.</summary>
    public string GetDescription() => Description.Value;

    /// <summary>Returns the moment the incident was reported.</summary>
    public DateTimeOffset GetReportedAt() => ReportedAt;
}