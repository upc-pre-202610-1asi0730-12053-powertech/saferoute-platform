namespace Powertech.Platform.Trip.Domain.Model.Commands;

/// <summary>
///     Command to prepare (create) a new trip session for a route and driver.
/// </summary>
/// <remarks>
///     Maps to the "Prepare Trip Session" step of the Trip Execution &amp; Monitoring event storming.
///     Carries primitive identifiers that the aggregate converts into Shared value objects.
/// </remarks>
/// <param name="OrganizationId">The organization that owns the trip.</param>
/// <param name="RouteId">The route the trip will be executed over.</param>
/// <param name="DriverId">The driver operating the trip.</param>
public record CreateTripCommand(
    Guid OrganizationId,
    Guid RouteId,
    Guid DriverId);
