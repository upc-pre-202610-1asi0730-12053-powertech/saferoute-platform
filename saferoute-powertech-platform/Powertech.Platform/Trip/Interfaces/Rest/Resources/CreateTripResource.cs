namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Input resource to prepare (create) a new trip.
/// </summary>
/// <param name="OrganizationId">The owning organization identifier (Guid as string).</param>
/// <param name="RouteId">The route the trip will run over (Guid as string).</param>
/// <param name="DriverId">The driver operating the trip (Guid as string).</param>
public record CreateTripResource(
    string OrganizationId,
    string RouteId,
    string DriverId);
