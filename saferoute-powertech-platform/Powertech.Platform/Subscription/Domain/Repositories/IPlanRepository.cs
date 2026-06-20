

using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Subscription.Domain.Model.Aggregates;

namespace Powertech.Platform.Subscription.Domain.Repositories;

public interface IPlanRepository : IBaseRepository<Plan>
{
    Task<Plan?> FindByPlanIdAsync(PlanId planId, CancellationToken cancellationToken);
}
