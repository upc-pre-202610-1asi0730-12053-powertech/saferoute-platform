using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using SubscriptionAggregate = Powertech.Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Powertech.Platform.Subscription.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<SubscriptionAggregate>
{
    Task<SubscriptionAggregate?> FindBySubscriptionIdAsync(SubscriptionId subscriptionId,
        CancellationToken cancellationToken);

    Task<IEnumerable<SubscriptionAggregate>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken);
}
