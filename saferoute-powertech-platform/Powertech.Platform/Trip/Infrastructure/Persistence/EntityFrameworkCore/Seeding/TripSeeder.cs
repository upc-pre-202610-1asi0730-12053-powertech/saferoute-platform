using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using RouteAggregate = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Trip bounded context with demo trips for the seeded active route: one trip
///     already completed and one pending for the day, so drivers and parents see history and
///     upcoming activity on first sign-in.
/// </summary>
public static class TripSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Set<TripAggregate>().AnyAsync(cancellationToken)) return;

        var activeState = new RouteState(RouteState.Active);
        var route = await context.Set<RouteAggregate>().Where(r => r.State == activeState)
            .FirstOrDefaultAsync(cancellationToken);
        var driver = await context.Set<Driver>().OrderBy(d => d.Email).FirstOrDefaultAsync(cancellationToken);
        if (route is null || driver is null) return;

        var completed = new TripAggregate(route.OrganizationId, route.Id, driver.Id);
        completed.Start();
        completed.Complete();
        context.Add(completed);

        var pending = new TripAggregate(route.OrganizationId, route.Id, driver.Id);
        context.Add(pending);

        await context.SaveChangesAsync(cancellationToken);
    }
}
