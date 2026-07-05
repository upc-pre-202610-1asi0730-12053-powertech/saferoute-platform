using Microsoft.EntityFrameworkCore;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;
using Powertech.Platform.Fleet.Domain.Repositories;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IRouteRepository" />.
/// </summary>
/// <remarks>
///     Inherits generic CRUD from <see cref="BaseRepository{TEntity}" /> and adds the Guid-identity
///     finders. The aggregate's stops, vehicle and assignment are mapped as owned types and are
///     therefore loaded automatically with the aggregate root.
/// </remarks>
/// <param name="context">The application database context.</param>
public class RouteRepository(AppDbContext context)
    : BaseRepository<Route>(context), IRouteRepository
{
    /// <inheritdoc />
    public override async Task<IEnumerable<Route>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await RoutesWithConfiguration()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Route?> FindByRouteIdAsync(RouteId routeId, CancellationToken cancellationToken)
    {
        return await RoutesWithConfiguration()
            .FirstOrDefaultAsync(route => route.Id == routeId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Route>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        return await RoutesWithConfiguration()
            .Where(route => route.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Route> RoutesWithConfiguration() =>
        Context.Set<Route>()
            .Include(route => route.Stops)
            .Include(route => route.Vehicle)
            .Include(route => route.Assignment)
            .AsSplitQuery();
}
