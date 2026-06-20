using Safer_Route_Platform.Shared.Domain.Model.Events;

namespace Safer_Route_Platform.Fleet.Domain.Model.Events;

/// <summary>Domain event raised when a vehicle is assigned to a route.</summary>
/// <param name="RouteId">The route the vehicle was assigned to.</param>
/// <param name="Plate">The assigned vehicle's plate.</param>
public record VehicleAssignedToRouteEvent(Guid RouteId, string Plate) : IEvent;