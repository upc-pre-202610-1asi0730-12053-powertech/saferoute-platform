namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to remove a stop from a route's sequence.
/// </summary>
/// <param name="RouteId">The route to remove the stop from.</param>
/// <param name="StopId">The identity of the stop to remove.</param>
public record RemoveStopCommand(Guid RouteId, Guid StopId);