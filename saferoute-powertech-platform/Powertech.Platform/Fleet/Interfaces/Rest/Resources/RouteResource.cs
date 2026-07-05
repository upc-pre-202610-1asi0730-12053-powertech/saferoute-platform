namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>
///     Output resource representing a route and its full configuration.
/// </summary>
/// <remarks>
///     Published contract consumed by the frontend Fleet &amp; Route Planning module. Identifiers are
///     exposed as strings (Guid). Optional parts (<paramref name="DepartureTime" />,
///     <paramref name="ServiceDays" />, <paramref name="Vehicle" />, <paramref name="Assignment" />)
///     are <c>null</c> while the route is still a draft being configured.
/// </remarks>
/// <param name="Id">The route identifier.</param>
/// <param name="OrganizationId">The owning organization identifier.</param>
/// <param name="Name">The route name.</param>
/// <param name="State">The lifecycle state (DRAFT, ACTIVE, INACTIVE).</param>
/// <param name="DepartureTime">The departure time in HH:mm, when set.</param>
/// <param name="ServiceDays">The configured service days, when set.</param>
/// <param name="Vehicle">The vehicle assigned to the route, when set.</param>
/// <param name="Assignment">The driver/children assignment, when set.</param>
/// <param name="Stops">The ordered stop sequence.</param>
public record RouteResource(
    string Id,
    string OrganizationId,
    string Name,
    string State,
    string? DepartureTime,
    IReadOnlyList<string> ServiceDays,
    VehicleResource? Vehicle,
    AssignmentResource? Assignment,
    IReadOnlyList<StopResource> Stops);
