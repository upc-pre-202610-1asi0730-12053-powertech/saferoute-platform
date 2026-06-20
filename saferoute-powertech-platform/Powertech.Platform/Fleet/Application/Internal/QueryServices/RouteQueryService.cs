using Powertech.Platform.Fleet.Application.QueryServices;
using Powertech.Platform.Fleet.Domain.Model.Queries;
using Powertech.Platform.Fleet.Domain.Repositories;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Application.Internal.QueryServices;

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