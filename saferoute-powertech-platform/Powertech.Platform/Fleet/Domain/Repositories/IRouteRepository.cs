using Route = Safer_Route_Platform.Fleet.Domain.Model.Aggregates.Route;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Repositories;

namespace Safer_Route_Platform.Fleet.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="Route" /> aggregate.
/// </summary>
/// <remarks>
///     Adds Guid-identity finders on top of the generic CRUD provided by
///     <see cref="IBaseRepository{TEntity}" />, since the Route aggregate uses a Guid-based identity.
/// </remarks>
public interface IRouteRepository : IBaseRepository<Route>
{
    /// <summary>
    ///     Finds a route by its <see cref="RouteId" />, eagerly loading its stops, vehicle and assignment.
    /// </summary>
    /// <param name="routeId">The route identity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The route if found; otherwise <c>null</c>.</returns>
    Task<Route?> FindByRouteIdAsync(RouteId routeId, CancellationToken cancellationToken);

    /// <summary>Lists all routes that belong to a given organization.</summary>
    /// <param name="organizationId">The organization identifier to filter by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The routes for the organization.</returns>
    Task<IEnumerable<Route>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken);
}