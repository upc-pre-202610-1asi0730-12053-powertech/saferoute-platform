using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Subscription.Domain.Repositories;
using SubscriptionAggregate = Powertech.Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Powertech.Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

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
