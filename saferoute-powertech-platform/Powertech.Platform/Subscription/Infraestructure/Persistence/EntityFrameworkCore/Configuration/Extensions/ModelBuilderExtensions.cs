using Microsoft.EntityFrameworkCore;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Subscription.Domain.Model.Aggregates;
using Safer_Route_Platform.Subscription.Domain.Model.ValueObjects;
using SubscriptionAggregate = Safer_Route_Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Safer_Route_Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySubscriptionConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plan>(plan =>
        {
            plan.ToTable("Plans");
            plan.HasKey(p => p.Id);
            plan.Property(p => p.Id).HasConversion(id => id.Identifier, value => new PlanId(value))
                .ValueGeneratedNever();
            plan.Property(p => p.Tier).HasConversion(tier => tier.Value, value => new PlanTier(value))
                .HasMaxLength(20).IsRequired();
            plan.Property(p => p.RouteQuota).HasConversion(quota => quota.MaxRoutes, value => new RouteQuota(value));
            plan.Property(p => p.DriverQuota).HasConversion(quota => quota.MaxDrivers, value => new DriverQuota(value));
            plan.Property(p => p.Price).HasPrecision(10, 2).IsRequired();
        });

        builder.Entity<SubscriptionAggregate>(subscription =>
        {
            subscription.ToTable("Subscriptions");
            subscription.HasKey(s => s.Id);
            subscription.Property(s => s.Id)
                .HasConversion(id => id.Identifier, value => new SubscriptionId(value))
                .ValueGeneratedNever();
            subscription.Property(s => s.OrganizationId)
                .HasConversion(id => id.Identifier, value => new OrganizationId(value));
            subscription.Property(s => s.PlanId).HasConversion(id => id.Identifier, value => new PlanId(value));
            subscription.Property(s => s.State)
                .HasConversion(state => state.Value, value => new SubscriptionState(value))
                .HasMaxLength(20)
                .IsRequired();
            subscription.Property(s => s.StartDate).IsRequired();
            subscription.Property(s => s.EndDate).IsRequired();

            // Foreign key to the subscribed plan (DB-level integrity; no navigation, no cascade).
            subscription.HasOne<Plan>().WithMany().HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
