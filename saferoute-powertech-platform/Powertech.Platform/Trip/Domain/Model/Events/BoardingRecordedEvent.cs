using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Trip.Domain.Model.Events;

/// <summary>
///     Domain event raised when a child's boarding status is recorded during a trip.
/// </summary>
/// <param name="TripId">The trip the boarding belongs to.</param>
/// <param name="ChildId">The child whose boarding was recorded.</param>
/// <param name="BoardingState">The resulting boarding state.</param>
public record BoardingRecordedEvent(Guid TripId, Guid ChildId, string BoardingState) : IEvent;