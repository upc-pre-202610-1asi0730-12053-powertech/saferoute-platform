namespace Powertech.Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to assign the operating driver to a route.
/// </summary>
/// <param name="RouteId">The route to assign the driver to.</param>
/// <param name="DriverId">The driver identifier.</param>
public record AssignDriverCommand(Guid RouteId, Guid DriverId);