using Safer_Route_Platform.Fleet.Application.QueryServices;
using Route = Safer_Route_Platform.Fleet.Domain.Model.Aggregates.Route;
using Safer_Route_Platform.Fleet.Domain.Model.Queries;
using Safer_Route_Platform.Fleet.Domain.Repositories;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;

namespace Safer_Route_Platform.Fleet.Application.Internal.QueryServices;

/// <summary>
///     Default implementation of <see cref="IRouteQueryService" />.
/// </summary>
/// <param name="routeRepository">The route repository abstraction.</param>


public class RouteQueryService(IRouteRepository routeRepository) : IRouteQueryService
{
    /// <inheritdoc />
    public async Task<Route?> Handle(GetRouteByIdQuery query, CancellationToken cancellationToken)
    {
        return await routeRepository.FindByRouteIdAsync(new RouteId(query.RouteId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Route>> Handle(GetAllRoutesQuery query, CancellationToken cancellationToken)
    {
        return await routeRepository.ListAsync(cancellationToken);
    }
    /// <inheritdoc />
    public async Task<IEnumerable<Route>> Handle(GetRoutesByOrganizationIdQuery query,
        CancellationToken cancellationToken)
    {
        return await routeRepository.FindByOrganizationIdAsync(new OrganizationId(query.OrganizationId),
            cancellationToken);
    }
}