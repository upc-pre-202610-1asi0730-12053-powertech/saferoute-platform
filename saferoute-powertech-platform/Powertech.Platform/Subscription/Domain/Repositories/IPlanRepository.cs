using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Repositories;
using Safer_Route_Platform.Subscription.Domain.Model.Aggregates;

namespace Safer_Route_Platform.Subscription.Domain.Repositories;

public interface IPlanRepository : IBaseRepository<Plan>
{
    Task<Plan?> FindByPlanIdAsync(PlanId planId, CancellationToken cancellationToken);
}
