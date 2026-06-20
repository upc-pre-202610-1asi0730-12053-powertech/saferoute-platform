namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to finalize the setup and activate a route.
/// </summary>
/// <remarks>Maps to the "RouteActivationFinalized" event-storming outcome.</remarks>
/// <param name="RouteId">The route to activate.</param>
public record ActivateRouteCommand(Guid RouteId);