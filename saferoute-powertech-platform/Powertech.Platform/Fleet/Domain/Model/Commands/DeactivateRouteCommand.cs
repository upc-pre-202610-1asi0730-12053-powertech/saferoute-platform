namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to deactivate an active route.
/// </summary>
/// <param name="RouteId">The route to deactivate.</param>
public record DeactivateRouteCommand(Guid RouteId);