using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Trip.Domain.Model.Events;

/// <summary>
///     Domain event raised when an incident is reported during a trip.
/// </summary>
/// <remarks>
///     This event is the integration hook the Notifications context listens to in order to notify
///     parents about an incident.
/// </remarks>
/// <param name="TripId">The trip the incident belongs to.</param>
/// <param name="Description">The incident description.</param>
/// <param name="ReportedAt">The moment the incident was reported.</param>
public record IncidentReportedEvent(Guid TripId, string Description, DateTimeOffset ReportedAt) : IEvent;