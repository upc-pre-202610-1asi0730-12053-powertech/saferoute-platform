using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Repositories;
using SubscriptionAggregate = Safer_Route_Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Safer_Route_Platform.Subscription.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<SubscriptionAggregate>
{
    Task<SubscriptionAggregate?> FindBySubscriptionIdAsync(SubscriptionId subscriptionId,
        CancellationToken cancellationToken);

    Task<IEnumerable<SubscriptionAggregate>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken);
}
