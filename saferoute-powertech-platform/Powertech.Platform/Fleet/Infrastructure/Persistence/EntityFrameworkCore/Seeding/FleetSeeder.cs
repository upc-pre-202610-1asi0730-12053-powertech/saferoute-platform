using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using RouteAggregate = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;
using FleetVehicle = Powertech.Platform.Fleet.Domain.Model.Entities.Vehicle;

namespace Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Fleet bounded context: the standalone vehicles catalog (raw table consumed by
///     the compatibility VehiclesController) and demo routes for the seed organization — one
///     active route fully configured (stops, vehicle, driver, children, schedule) and one draft.
/// </summary>
public static class FleetSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await EnsureVehiclesCatalogAsync(context, cancellationToken);

        if (await context.Set<RouteAggregate>().AnyAsync(cancellationToken)) return;

        var organizationId = new OrganizationId(IamSeeder.SeedOrganizationId);
        var driver = await context.Set<Driver>().OrderBy(d => d.Email).FirstOrDefaultAsync(cancellationToken);
        var children = (await context.Set<Parent>().ToListAsync(cancellationToken))
            .SelectMany(parent => parent.Children).ToList();

        var route = new RouteAggregate(organizationId, "Ruta San Martín — Turno Mañana");
        route.AddStop("Colegio San Martín", new Coordinates(-12.0464, -77.0428));
        route.AddStop("Parque Kennedy", new Coordinates(-12.1211, -77.0297));
        route.AddStop("Plaza San Miguel", new Coordinates(-12.0776, -77.0824));
        route.AssignVehicle(new FleetVehicle(organizationId, "ABC-123", "Sprinter 416", "Mercedes-Benz", 20));
        if (driver is not null) route.AssignDriver(driver.Id);
        foreach (var child in children) route.AssignChild(child.Id);
        route.DefineServiceDays(new ServiceDays(new[] { "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY" }));
        route.SetDepartureTime(new DepartureTime("07:00"));
        if (driver is not null && children.Count > 0) route.Activate();
        context.Entry(route).Property(r => r.Id).CurrentValue =
            new RouteId(Guid.Parse("f0000000-0000-0000-0000-000000000001"));
        context.Add(route);

        var draft = new RouteAggregate(organizationId, "Ruta Miraflores — Turno Tarde");
        draft.AddStop("Colegio San Martín", new Coordinates(-12.0464, -77.0428));
        draft.AddStop("Óvalo Gutiérrez", new Coordinates(-12.1080, -77.0505));
        context.Entry(draft).Property(r => r.Id).CurrentValue =
            new RouteId(Guid.Parse("f0000000-0000-0000-0000-000000000002"));
        context.Add(draft);

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     The vehicles catalog lives outside the EF model (VehiclesController queries it with
    ///     raw SQL), so the table is created here when missing and seeded when empty.
    /// </summary>
    private static async Task EnsureVehiclesCatalogAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS vehicles (
                id varchar(36) NOT NULL PRIMARY KEY,
                organization_id varchar(36) NOT NULL,
                plate varchar(20) NOT NULL,
                model varchar(80) NOT NULL,
                capacity int NOT NULL,
                status varchar(20) NOT NULL
            );
            """, cancellationToken);

        var count = await context.Database
            .SqlQueryRaw<long>("SELECT COUNT(*) AS Value FROM vehicles")
            .SingleAsync(cancellationToken);
        if (count > 0) return;

        var organizationId = IamSeeder.SeedOrganizationId.ToString();
        await context.Database.ExecuteSqlRawAsync("""
            INSERT INTO vehicles (id, organization_id, plate, model, capacity, status) VALUES
            ('a1000000-0000-0000-0000-000000000001', {0}, 'ABC-123', 'Mercedes-Benz Sprinter 416', 20, 'ACTIVE'),
            ('a1000000-0000-0000-0000-000000000002', {0}, 'DEF-456', 'Toyota Hiace', 15, 'ACTIVE');
            """, [organizationId], cancellationToken);
    }
}
