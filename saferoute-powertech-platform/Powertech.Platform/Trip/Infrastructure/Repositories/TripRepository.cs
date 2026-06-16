using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Trip.Domain.Repositories;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Infrastructure.Repositories;

/// <summary>
///     EF Core implementation of <see cref="ITripRepository" />.
/// </summary>
/// <remarks>
///     Inherits the generic CRUD operations from <see cref="BaseRepository{TEntity}" /> and adds the
///     Guid-identity finders declared by the domain contract. The aggregate's child collections
///     (<c>Attendances</c> and <c>Incidents</c>) are mapped as owned types, so EF Core loads them
///     automatically together with the aggregate root.
/// </remarks>
/// <param name="context">The application database context.</param>
public class TripRepository(AppDbContext context)
    : BaseRepository<TripAggregate>(context), ITripRepository
{
    /// <inheritdoc />
    public async Task<TripAggregate?> FindByTripIdAsync(TripId tripId, CancellationToken cancellationToken)
    {
        return await Context.Set<TripAggregate>()
            .FirstOrDefaultAsync(trip => trip.Id == tripId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TripAggregate>> FindByRouteIdAsync(RouteId routeId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<TripAggregate>()
            .Where(trip => trip.RouteId == routeId)
            .ToListAsync(cancellationToken);
    }
}