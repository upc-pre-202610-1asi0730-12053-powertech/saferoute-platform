using Microsoft.EntityFrameworkCore;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Safer_Route_Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Safer_Route_Platform.Subscription.Domain.Repositories;
using SubscriptionAggregate = Safer_Route_Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Safer_Route_Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<SubscriptionAggregate>(context), ISubscriptionRepository
{
    public Task<SubscriptionAggregate?> FindBySubscriptionIdAsync(SubscriptionId subscriptionId,
        CancellationToken cancellationToken) =>
        Context.Set<SubscriptionAggregate>()
            .FirstOrDefaultAsync(subscription => subscription.Id == subscriptionId, cancellationToken);

    public async Task<IEnumerable<SubscriptionAggregate>> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken) =>
        await Context.Set<SubscriptionAggregate>()
            .Where(subscription => subscription.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
}
