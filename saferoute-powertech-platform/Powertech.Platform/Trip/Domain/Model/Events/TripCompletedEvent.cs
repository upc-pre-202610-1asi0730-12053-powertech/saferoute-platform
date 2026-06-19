using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Trip.Domain.Model.Events;

/// <summary>
///     Domain event raised when a trip transitions from <c>IN_PROGRESS</c> to <c>COMPLETED</c>.
/// </summary>
/// <param name="TripId">The identifier of the completed trip.</param>
/// <param name="CompletedAt">The moment the trip completed.</param>
public record TripCompletedEvent(Guid TripId, DateTimeOffset CompletedAt) : IEvent;
