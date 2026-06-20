namespace Powertech.Platform.Fleet.Domain.Model.Queries;

/// <summary>Query to retrieve a single route by its unique identifier.</summary>
/// <param name="RouteId">The identifier of the route to retrieve.</param>


public record GetRouteByIdQuery(Guid RouteId);