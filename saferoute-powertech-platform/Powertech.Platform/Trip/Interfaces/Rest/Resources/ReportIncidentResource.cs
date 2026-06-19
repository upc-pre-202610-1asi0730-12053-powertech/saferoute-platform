namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Input resource to report an incident during a trip.
/// </summary>
/// <param name="Description">The free-text description of the incident (10–500 characters).</param>
public record ReportIncidentResource(string Description);
