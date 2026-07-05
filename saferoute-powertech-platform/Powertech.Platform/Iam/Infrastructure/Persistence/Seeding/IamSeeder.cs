using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Iam.Application.Internal.OutboundServices;
using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Iam.Infrastructure.Persistence.Seeding;

/// <summary>
///     Seeds the Iam bounded context with the initial organization and demo user accounts.
/// </summary>
/// <remarks>
///     Runs once on startup right after migrations: when the organizations table already has
///     rows the seeder is a no-op, so redeploys (e.g. Azure App Service container restarts)
///     never duplicate data. The identifiers are fixed so the frontend demo flows and other
///     seeded contexts can reference them deterministically.
/// </remarks>
public static class IamSeeder
{
    public static readonly Guid SeedOrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(AppDbContext context, IHashingService hashingService,
        CancellationToken cancellationToken = default)
    {
        if (await context.Set<Organization>().AnyAsync(cancellationToken)) return;

        var organization = new Organization(new OrganizationName("Transportes Escolares Lima Norte S.A.C."));
        context.Entry(organization).Property(o => o.Id).CurrentValue = new OrganizationId(SeedOrganizationId);
        context.Add(organization);

        AddUser(context, hashingService, "b0000000-0000-0000-0000-000000000001",
            "Nickolas", "Quispe", "admin@saferoute.pe", "admin123", RoleTier.Admin);
        AddUser(context, hashingService, "b0000000-0000-0000-0000-000000000002",
            "Carlos", "Ramirez", "driver@saferoute.pe", "driver123", RoleTier.Driver);
        AddUser(context, hashingService, "b0000000-0000-0000-0000-000000000003",
            "Rosita", "Nery", "parent@saferoute.pe", "parent123", RoleTier.Parent);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static void AddUser(AppDbContext context, IHashingService hashingService, string id,
        string firstName, string lastName, string email, string password, string roleTier)
    {
        var user = new User(
            new OrganizationId(SeedOrganizationId),
            new FullName(firstName, lastName),
            new Email(email),
            new PasswordHash(hashingService.HashPassword(password)),
            new RoleTier(roleTier));
        context.Entry(user).Property(u => u.Id).CurrentValue = new UserId(Guid.Parse(id));
        context.Add(user);
    }
}