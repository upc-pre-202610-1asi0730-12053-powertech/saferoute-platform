namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

public record UpdateRouteResource(
    string? Name,
    string? Status,
    string? RouteState,
    string? ScheduledStartTime,
    string? DepartureTime,
    IReadOnlyList<string>? ServiceDays,
    IReadOnlyList<RouteWaypointResource>? Waypoints,
    string? VehicleId,
    string? VehiclePlate,
    string? VehicleModel,
    string? VehicleBrand,
    int? VehicleCapacity,
    string? DriverId,
    IReadOnlyList<string>? StudentIds);

public record RouteWaypointResource(
    string? Id,
    string? Name,
    double? Lat,
    double? Lng,
    double? Latitude,
    double? Longitude,
    int? Order,
    IReadOnlyList<string>? StudentIds);
