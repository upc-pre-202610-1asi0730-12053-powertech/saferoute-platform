using Microsoft.EntityFrameworkCore;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Safer_Route_Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Safer_Route_Platform.Subscription.Domain.Model.Aggregates;
using Safer_Route_Platform.Subscription.Domain.Repositories;

namespace Safer_Route_Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PlanRepository(AppDbContext context) : BaseRepository<Plan>(context), IPlanRepository
{
    public Task<Plan?> FindByPlanIdAsync(PlanId planId, CancellationToken cancellationToken) =>
        Context.Set<Plan>().FirstOrDefaultAsync(plan => plan.Id == planId, cancellationToken);
}
