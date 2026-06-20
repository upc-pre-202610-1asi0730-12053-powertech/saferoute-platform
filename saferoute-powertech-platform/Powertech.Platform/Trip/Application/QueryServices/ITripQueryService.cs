using Powertech.Platform.Trip.Domain.Model.Queries;

namespace Powertech.Platform.Trip.Application.QueryServices;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

/// <summary>
///     Application service that handles the read operations (queries) of the Trip context.
/// </summary>
public interface ITripQueryService
{
    /// <summary>Handles retrieving a single trip by its identifier.</summary>
    /// <param name="query">The query carrying the trip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The trip if found; otherwise <c>null</c>.</returns>
    Task<TripAggregate?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken);
    
    /// <summary>Handles retrieving all trips.</summary>
    Task<IEnumerable<TripAggregate>> Handle(GetAllTripsQuery query, CancellationToken cancellationToken);
    
    /// <summary>Handles retrieving all trips for a given route.</summary>
    Task<IEnumerable<TripAggregate>> Handle(GetTripsByRouteIdQuery query, CancellationToken cancellationToken);
    
}
