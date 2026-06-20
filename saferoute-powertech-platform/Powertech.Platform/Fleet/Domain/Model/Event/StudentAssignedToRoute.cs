using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Fleet.Domain.Model.Event;

/// <summary>Domain event raised when a child (student) is assigned to a route.</summary>
/// <param name="RouteId">The route the child was assigned to.</param>
/// <param name="ChildId">The assigned child.</param>
public record StudentAssignedToRouteEvent(Guid RouteId, Guid ChildId) : IEvent;