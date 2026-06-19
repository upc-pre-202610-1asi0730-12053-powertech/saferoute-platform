using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Trip.Domain.Model.Events;

/// <summary>
///     Domain event raised when a trip transitions from <c>PENDING</c> to <c>IN_PROGRESS</c>.
/// </summary>
/// <param name="TripId">The identifier of the started trip.</param>
/// <param name="StartedAt">The moment the trip started.</param>
public record TripStartedEvent(Guid TripId, DateTimeOffset StartedAt) : IEvent;