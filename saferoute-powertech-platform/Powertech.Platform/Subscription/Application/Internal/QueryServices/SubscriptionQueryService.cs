using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Subscription.Application.QueryServices;
using Powertech.Platform.Subscription.Domain.Model.Aggregates;
using Powertech.Platform.Subscription.Domain.Model.Queries;
using Powertech.Platform.Subscription.Domain.Repositories;
using SubscriptionAggregate = Powertech.Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Powertech.Platform.Subscription.Application.Internal.QueryServices;

public class SubscriptionQueryService(IPlanRepository planRepository, ISubscriptionRepository subscriptionRepository)
    : ISubscriptionQueryService
{
    public Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken) =>
        planRepository.FindByPlanIdAsync(new PlanId(query.PlanId), cancellationToken);

    public async Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken) =>
        await planRepository.ListAsync(cancellationToken);

    public Task<SubscriptionAggregate?> Handle(GetSubscriptionByIdQuery query, CancellationToken cancellationToken) =>
        subscriptionRepository.FindBySubscriptionIdAsync(new SubscriptionId(query.SubscriptionId), cancellationToken);

    public async Task<IEnumerable<SubscriptionAggregate>> Handle(GetAllSubscriptionsQuery query,
        CancellationToken cancellationToken) =>
        await subscriptionRepository.ListAsync(cancellationToken);

    public Task<IEnumerable<SubscriptionAggregate>> Handle(GetSubscriptionsByOrganizationIdQuery query,
        CancellationToken cancellationToken) =>
        subscriptionRepository.FindByOrganizationIdAsync(new OrganizationId(query.OrganizationId), cancellationToken);
}
