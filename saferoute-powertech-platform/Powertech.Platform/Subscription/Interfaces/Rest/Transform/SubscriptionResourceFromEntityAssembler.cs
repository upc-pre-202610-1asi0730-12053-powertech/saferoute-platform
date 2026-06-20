using Powertech.Platform.Subscription.Domain.Model.Aggregates;
using Powertech.Platform.Subscription.Interfaces.Rest.Resources;
using SubscriptionAggregate = Powertech.Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Powertech.Platform.Subscription.Interfaces.Rest.Transform;

public static class SubscriptionResourceFromEntityAssembler
{
    public static PlanResource ToResourceFromEntity(Plan plan) =>
        new(plan.Id.Identifier, plan.Tier.Value, plan.RouteQuota.MaxRoutes, plan.DriverQuota.MaxDrivers, plan.Price);

    public static SubscriptionResource ToResourceFromEntity(SubscriptionAggregate subscription) =>
        new(subscription.Id.Identifier, subscription.OrganizationId.Identifier, subscription.PlanId.Identifier,
            subscription.State.Value, subscription.StartDate, subscription.EndDate, subscription.GetRemainingDays());
}
