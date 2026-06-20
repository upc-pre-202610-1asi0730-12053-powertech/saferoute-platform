using Safer_Route_Platform.Subscription.Domain.Model.Aggregates;
using Safer_Route_Platform.Subscription.Domain.Model.Queries;
using SubscriptionAggregate = Safer_Route_Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Safer_Route_Platform.Subscription.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken);

    Task<SubscriptionAggregate?> Handle(GetSubscriptionByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<SubscriptionAggregate>> Handle(GetAllSubscriptionsQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<SubscriptionAggregate>> Handle(GetSubscriptionsByOrganizationIdQuery query,
        CancellationToken cancellationToken);
}
