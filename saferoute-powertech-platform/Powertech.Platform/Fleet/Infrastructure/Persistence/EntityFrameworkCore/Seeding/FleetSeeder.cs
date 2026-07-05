using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Fleet.Domain.Model;
using Powertech.Platform.Fleet.Domain.Model.Entities;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using RouteAggregate = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;
using FleetVehicle = Powertech.Platform.Fleet.Domain.Model.Entities.Vehicle;

namespace Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Fleet bounded context: the standalone vehicles catalog and demo routes for the
///     seed organization. The route seed is idempotent and repairs previously seeded incomplete
///     local data.
/// </summary>
public static class FleetSeeder
{
    private static readonly Guid MorningRouteId = Guid.Parse("f0000000-0000-0000-0000-000000000001");
    private static readonly Guid DraftRouteId = Guid.Parse("f0000000-0000-0000-0000-000000000002");

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await EnsureVehiclesCatalogAsync(context, cancellationToken);

        var organizationId = new OrganizationId(IamSeeder.SeedOrganizationId);
        var driver = await context.Set<Driver>().OrderBy(driver => driver.Email).FirstOrDefaultAsync(cancellationToken);
        var children = (await context.Set<Parent>()
                .Include(parent => parent.Children)
                .ToListAsync(cancellationToken))
            .SelectMany(parent => parent.Children)
            .ToList();

        await EnsureMorningRouteAsync(context, organizationId, driver, children, cancellationToken);
        await EnsureDraftRouteAsync(context, organizationId, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureMorningRouteAsync(AppDbContext context, OrganizationId organizationId,
        Driver? driver, IReadOnlyCollection<Child> children, CancellationToken cancellationToken)
    {
        var routeId = new RouteId(MorningRouteId);
        var route = await FindSeedRouteAsync(context, routeId, cancellationToken);
        if (route is null)
        {
            route = new RouteAggregate(organizationId, "Ruta San Martin - Turno Manana");
            context.Entry(route).Property(currentRoute => currentRoute.Id).CurrentValue = routeId;
            context.Add(route);
        }

        if (!NeedsMorningRouteRepair(route, children.Count)) return;

        var assignment = driver is null
            ? null
            : new Assignment(AssignmentId.New(), driver.Id, children.Select(child => child.Id));
        var state = driver is not null && children.Count > 0
            ? new RouteState(RouteState.Active)
            : RouteState.CreateDraft();

        route.ReplaceConfiguration(
            "Ruta San Martin - Turno Manana",
            state,
            new DepartureTime("07:00"),
            new ServiceDays(["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"]),
            [
                new Stop("Colegio San Martin", new Coordinates(-12.0464, -77.0428), new StopOrder(1)),
                new Stop("Parque Kennedy", new Coordinates(-12.1211, -77.0297), new StopOrder(2)),
                new Stop("Plaza San Miguel", new Coordinates(-12.0776, -77.0824), new StopOrder(3))
            ],
            new FleetVehicle(new VehicleId(Guid.Parse("a1000000-0000-0000-0000-000000000001")),
                organizationId, "ABC-123", "Sprinter 416", "Mercedes-Benz", 20),
            assignment);
    }

    private static async Task EnsureDraftRouteAsync(AppDbContext context, OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        var routeId = new RouteId(DraftRouteId);
        var route = await FindSeedRouteAsync(context, routeId, cancellationToken);
        if (route is null)
        {
            route = new RouteAggregate(organizationId, "Ruta Miraflores - Turno Tarde");
            context.Entry(route).Property(currentRoute => currentRoute.Id).CurrentValue = routeId;
            context.Add(route);
        }

        if (route.GetTotalStops() >= 2) return;

        route.ReplaceConfiguration(
            "Ruta Miraflores - Turno Tarde",
            RouteState.CreateDraft(),
            null,
            new ServiceDays([]),
            [
                new Stop("Colegio San Martin", new Coordinates(-12.0464, -77.0428), new StopOrder(1)),
                new Stop("Ovalo Gutierrez", new Coordinates(-12.1080, -77.0505), new StopOrder(2))
            ],
            null,
            null);
    }

    private static Task<RouteAggregate?> FindSeedRouteAsync(AppDbContext context, RouteId routeId,
        CancellationToken cancellationToken) =>
        context.Set<RouteAggregate>()
            .Include(route => route.Stops)
            .Include(route => route.Vehicle)
            .Include(route => route.Assignment)
            .AsSplitQuery()
            .FirstOrDefaultAsync(route => route.Id == routeId, cancellationToken);

    private static bool NeedsMorningRouteRepair(RouteAggregate route, int availableChildrenCount) =>
        route.GetTotalStops() < 2 ||
        route.Vehicle is null ||
        route.Assignment is null ||
        availableChildrenCount > 0 && route.Assignment.GetChildCount() == 0 ||
        route.ServiceDays is null ||
        !route.ServiceDays.IsValid() ||
        route.DepartureTime is null;

    private static async Task EnsureVehiclesCatalogAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Set<VehicleCatalogItem>().AnyAsync(cancellationToken)) return;

        var organizationId = IamSeeder.SeedOrganizationId;
        await context.Set<VehicleCatalogItem>().AddRangeAsync([
            new VehicleCatalogItem
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                OrganizationId = organizationId,
                Plate = "ABC-123",
                Model = "Mercedes-Benz Sprinter 416",
                Capacity = 20,
                Status = "ACTIVE"
            },
            new VehicleCatalogItem
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                OrganizationId = organizationId,
                Plate = "DEF-456",
                Model = "Toyota Hiace",
                Capacity = 15,
                Status = "ACTIVE"
            }
        ], cancellationToken);
    }
}
