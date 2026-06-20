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
    public async Task<Route?> FindByRouteIdAsync(RouteId routeId, CancellationToken cancellationToken)
    {
        return await Context.Set<Route>()
            .FirstOrDefaultAsync(route => route.Id == routeId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Route>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Route>()
            .Where(route => route.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }
}
