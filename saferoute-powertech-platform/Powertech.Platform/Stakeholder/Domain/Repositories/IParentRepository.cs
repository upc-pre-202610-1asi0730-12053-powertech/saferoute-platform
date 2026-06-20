using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;

namespace Powertech.Platform.Stakeholder.Domain.Repositories;

public interface IParentRepository : IBaseRepository<Parent>
{
    Task<Parent?> FindByParentIdAsync(ParentId parentId, CancellationToken cancellationToken);
}
