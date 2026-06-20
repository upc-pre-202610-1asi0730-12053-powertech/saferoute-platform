using Safer_Route_Platform.Shared.Domain.Model.Events;

namespace Safer_Route_Platform.Fleet.Domain.Model.Events;

/// <summary>Domain event raised when a route's setup is finalized and it becomes active.</summary>
/// <param name="RouteId">The identifier of the activated route.</param>
public record RouteActivatedEvent(Guid RouteId) : IEvent;