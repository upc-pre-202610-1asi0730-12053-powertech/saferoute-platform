namespace Powertech.Platform.Trip.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all trips executed over a given route.
/// </summary>
/// <param name="RouteId">The route identifier to filter trips by.</param>
public record GetTripsByRouteIdQuery(Guid RouteId);
