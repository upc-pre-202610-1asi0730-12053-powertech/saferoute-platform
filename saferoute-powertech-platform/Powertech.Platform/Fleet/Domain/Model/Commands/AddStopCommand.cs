namespace Safer_Route_Platform_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to append a stop (waypoint) to a route's sequence.
/// </summary>
/// <remarks>Maps to the "Pick Waypoints" event-storming command.</remarks>
/// <param name="RouteId">The route to add the stop to.</param>
/// <param name="Name">The stop name.</param>
/// <param name="Latitude">The stop latitude.</param>
/// <param name="Longitude">The stop longitude.</param>
public record AddStopCommand(Guid RouteId, string Name, double Latitude, double Longitude);