using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="TripAggregate" /> aggregate.
/// </summary>
/// <remarks>
///     Declares the persistence operations required by the Trip domain. The implementation lives in
///     the Infrastructure layer. Because the Trip aggregate uses a Guid-based identity, this
///     contract adds identity-typed finders on top of the generic CRUD provided by
///     <see cref="IBaseRepository{TEntity}" />.
/// </remarks>
public interface ITripRepository : IBaseRepository<TripAggregate>
{
    /// <summary>
    ///     Finds a trip by its <see cref="TripId" />, eagerly loading its attendances and incidents.
    /// </summary>
    /// <param name="tripId">The trip identity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The trip if found; otherwise <c>null</c>.</returns>
    Task<TripAggregate?> FindByTripIdAsync(TripId tripId, CancellationToken cancellationToken);

    /// <summary>
    ///     Lists all trips executed over a given route.
    /// </summary>
    /// <param name="routeId">The route identifier to filter by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The trips for the route.</returns>
    Task<IEnumerable<TripAggregate>> FindByRouteIdAsync(RouteId routeId, CancellationToken cancellationToken);
}
