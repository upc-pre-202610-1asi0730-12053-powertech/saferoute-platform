namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to assign a child (student) to a route.
/// </summary>
/// <remarks>Maps to the "Assign Students to Route" event-storming command,
/// producing a <c>StudentAssignedToRoute</c> event.</remarks>
/// <param name="RouteId">The route to assign the child to.</param>
/// <param name="ChildId">The child identifier.</param>


public record AssignStudentsToRouteCommand(Guid RouteId, Guid ChildId);