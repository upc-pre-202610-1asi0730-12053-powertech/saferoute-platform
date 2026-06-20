using Powertech.Platform.Fleet.Domain.Model.Queries;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Application.QueryServices;

/// <summary>
///     Application service that handles the read operations (queries) of the Fleet context.
/// </summary>
public interface IRouteQueryService
{
    /// <summary>Handles retrieving a single route by its identifier.</summary>
    Task<Route?> Handle(GetRouteByIdQuery query, CancellationToken cancellationToken);

    /// <summary>Handles retrieving all routes.</summary>
    Task<IEnumerable<Route>> Handle(GetAllRoutesQuery query, CancellationToken cancellationToken);

    /// <summary>Handles retrieving all routes that belong to an organization.</summary>
    Task<IEnumerable<Route>> Handle(GetRoutesByOrganizationIdQuery query, CancellationToken cancellationToken);
}
