using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Repositories;

namespace Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ParentRepository(AppDbContext context) : BaseRepository<Parent>(context), IParentRepository
{
    public Task<Parent?> FindByParentIdAsync(ParentId parentId, CancellationToken cancellationToken) =>
        Context.Set<Parent>().FirstOrDefaultAsync(parent => parent.Id == parentId, cancellationToken);
}
