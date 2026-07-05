using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Subscription.Domain.Model.Aggregates;
using Powertech.Platform.Subscription.Domain.Model.ValueObjects;

namespace Powertech.Platform.Subscription.Infraestructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Subscription bounded context with the three commercial plan tiers.
/// </summary>
/// <remarks>
///     Runs once on startup after migrations; a no-op when plans already exist. The plan
///     identifiers are fixed so the frontend checkout can resolve them deterministically.
///     Quotas and prices mirror the plan catalog shown on the landing page.
/// </remarks>
public static class SubscriptionSeeder
{
    private static readonly Guid CompletePlanId = Guid.Parse("c0000000-0000-0000-0000-000000000003");

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Set<Plan>().AnyAsync(cancellationToken))
        {
            AddPlan(context, "c0000000-0000-0000-0000-000000000001", PlanTier.Basic, 2, 2, 9.99m);
            AddPlan(context, "c0000000-0000-0000-0000-000000000002", PlanTier.Intermediate, 6, 6, 24.99m);
            AddPlan(context, CompletePlanId.ToString(), PlanTier.Complete, 20, 20, 49.99m);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<Domain.Model.Aggregates.Subscription>().AnyAsync(cancellationToken))
        {
            // Active subscription for the seed organization on the COMPLETE tier.
            var subscription = new Domain.Model.Aggregates.Subscription(
                new OrganizationId(Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding.IamSeeder
                    .SeedOrganizationId),
                new PlanId(CompletePlanId),
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddDays(30));
            context.Entry(subscription).Property(s => s.Id).CurrentValue =
                new SubscriptionId(Guid.Parse("50000000-0000-0000-0000-000000000001"));
            context.Add(subscription);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static void AddPlan(AppDbContext context, string id, string tier, int maxRoutes, int maxDrivers,
        decimal price)
    {
        var plan = new Plan(new PlanTier(tier), new RouteQuota(maxRoutes), new DriverQuota(maxDrivers), price);
        context.Entry(plan).Property(p => p.Id).CurrentValue = new PlanId(Guid.Parse(id));
        context.Add(plan);
    }
}
