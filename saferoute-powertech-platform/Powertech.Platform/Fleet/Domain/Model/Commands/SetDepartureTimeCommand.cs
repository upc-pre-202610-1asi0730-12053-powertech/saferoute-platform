namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to set the departure time of a route.
/// </summary>
/// <remarks>Maps to the "Set Departure Time" event-storming command, producing a <c>DepartureTimeSet</c> event.</remarks>
/// <param name="RouteId">The route to configure.</param>
/// <param name="DepartureTime">The departure time in HH:mm format.</param>
public record SetDepartureTimeCommand(Guid RouteId, string DepartureTime);