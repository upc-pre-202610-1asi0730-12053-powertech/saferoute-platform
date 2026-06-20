using Powertech.Platform.Shared.Domain.Model.Events;

namespace Powertech.Platform.Fleet.Domain.Model.Events;

/// <summary>Domain event raised when a new route is defined.</summary>
/// <param name="RouteId">The identifier of the defined route.</param>
/// <param name="OrganizationId">The owning organization.</param>
/// <param name="Name">The route name.</param>
public record RouteDefinedEvent(Guid RouteId, Guid OrganizationId, string Name) : IEvent;