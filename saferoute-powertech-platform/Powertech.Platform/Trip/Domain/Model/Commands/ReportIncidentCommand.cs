namespace Powertech.Platform.Trip.Domain.Model.Commands;

/// <summary>
///     Command to report an incident during an in-progress trip.
/// </summary>
/// <remarks>Maps to the "Report Incident" event-storming command, producing an <c>IncidentReported</c> event.</remarks>
/// <param name="TripId">The trip the incident belongs to.</param>
/// <param name="Description">The free-text description of the incident.</param>
public record ReportIncidentCommand(
    Guid TripId,
    string Description);