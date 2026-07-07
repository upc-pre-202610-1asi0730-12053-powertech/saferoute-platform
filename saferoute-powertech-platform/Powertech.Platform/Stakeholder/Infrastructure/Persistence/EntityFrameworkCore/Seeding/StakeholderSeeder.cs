using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Commands;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Stakeholder bounded context with demo drivers, parents and children for the
///     seed organization. The first driver and parent are linked to the seeded IAM accounts
///     (driver@saferoute.pe / parent@saferoute.pe) through their shared UserId.
/// </summary>
public static class StakeholderSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var organizationId = IamSeeder.SeedOrganizationId;

        await AddDriverIfMissingAsync(context, "d0000000-0000-0000-0000-000000000001", organizationId,
            "b0000000-0000-0000-0000-000000000002", "Carlos", "Ramirez", "driver@saferoute.pe",
            "987654321", "Q12345678", cancellationToken);
        await AddDriverIfMissingAsync(context, "d0000000-0000-0000-0000-000000000002", organizationId,
            "b0000000-0000-0000-0000-000000000004", "Luis", "Torres", "luis.torres@saferoute.pe",
            "987112233", "Q87654321", cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000001", organizationId,
            "b0000000-0000-0000-0000-000000000003", "Rosita", "Nery", "parent@saferoute.pe",
            "999888777", [new Child(new FullName("Mateo", "Nery"), 7),
                new Child(new FullName("Luciana", "Nery"), 9)], cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000002", organizationId,
            "b0000000-0000-0000-0000-000000000005", "Jorge", "Salas", "jorge.salas@gmail.com",
            "998877665", [new Child(new FullName("Diego", "Salas"), 8)], cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000003", organizationId,
            "b0000000-0000-0000-0000-000000000006", "Mariana", "Flores", "mariana.flores@gmail.com",
            "997766554", [new Child(new FullName("Camila", "Flores"), 6),
                new Child(new FullName("Nicolas", "Flores"), 10)], cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000004", organizationId,
            "b0000000-0000-0000-0000-000000000007", "Pablo", "Mendoza", "pablo.mendoza@gmail.com",
            "996655443", [new Child(new FullName("Valeria", "Mendoza"), 7)], cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000005", organizationId,
            "b0000000-0000-0000-0000-000000000008", "Elena", "Castillo", "elena.castillo@gmail.com",
            "995544332", [new Child(new FullName("Sebastian", "Castillo"), 8),
                new Child(new FullName("Ana", "Castillo"), 11)], cancellationToken);

        await AddParentIfMissingAsync(context, "e0000000-0000-0000-0000-000000000006", organizationId,
            "b0000000-0000-0000-0000-000000000009", "Victor", "Rojas", "victor.rojas@gmail.com",
            "994433221", [new Child(new FullName("Sofia", "Rojas"), 9)], cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddDriverIfMissingAsync(AppDbContext context, string id, Guid organizationId,
        string userId, string firstName, string lastName, string email, string phone, string license,
        CancellationToken cancellationToken)
    {
        var driverId = new DriverId(Guid.Parse(id));
        var normalizedEmail = new Email(email, false);
        if (await context.Set<Driver>().AnyAsync(driver =>
                driver.Id == driverId || driver.Email == normalizedEmail, cancellationToken))
            return;

        var driver = new Driver(new CreateDriverCommand(organizationId, Guid.Parse(userId), firstName, lastName,
            email, phone, license));
        context.Entry(driver).Property(d => d.Id).CurrentValue = driverId;
        context.Add(driver);
    }

    private static async Task AddParentIfMissingAsync(AppDbContext context, string id, Guid organizationId,
        string userId, string firstName, string lastName, string email, string phone, IEnumerable<Child> children,
        CancellationToken cancellationToken)
    {
        var parentId = new ParentId(Guid.Parse(id));
        var normalizedEmail = new Email(email, false);
        if (await context.Set<Parent>().AnyAsync(parent =>
                parent.Id == parentId || parent.Email == normalizedEmail, cancellationToken))
            return;

        var parent = new Parent(new CreateParentCommand(organizationId,
            Guid.Parse(userId), firstName, lastName, email, phone));
        foreach (var child in children)
            parent.AddChild(child);

        context.Entry(parent).Property(p => p.Id).CurrentValue = parentId;
        context.Add(parent);
    }
}
