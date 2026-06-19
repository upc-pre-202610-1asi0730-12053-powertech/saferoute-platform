namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Output resource representing an incident reported during a trip.
/// </summary>
/// <param name="Id">The incident identifier (Guid as string).</param>
/// <param name="Description">The incident description.</param>
/// <param name="ReportedAt">The moment the incident was reported.</param>
public record IncidentResource(
    string Id,
    string Description,
    DateTimeOffset ReportedAt);
