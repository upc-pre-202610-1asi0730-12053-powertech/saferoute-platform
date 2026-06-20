namespace Safer_Route_Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to remove a child (student) from a route.
/// </summary>
/// <param name="RouteId">The route to remove the child from.</param>
/// <param name="ChildId">The child identifier.</param>
public record RemoveStudentFromRouteCommand(Guid RouteId, Guid ChildId);