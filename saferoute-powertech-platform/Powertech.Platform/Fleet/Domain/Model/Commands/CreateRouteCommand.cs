namespace Powertech.Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to define (create) a new route in the draft state.
/// </summary>
/// <remarks>Maps to the "Define Route" event-storming command, producing a <c>RouteDefined</c> event.</remarks>
/// <param name="OrganizationId">The owning organization identifier.</param>
/// <param name="Name">The route name.</param>
public record CreateRouteCommand(Guid OrganizationId, string Name);