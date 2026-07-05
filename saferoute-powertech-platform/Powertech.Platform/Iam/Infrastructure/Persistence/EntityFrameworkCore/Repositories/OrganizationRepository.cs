using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Iam.Domain.Repositories;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class OrganizationRepository(AppDbContext context) : BaseRepository<Organization>(context),
    IOrganizationRepository
{
    public Task<Organization?> FindByOrganizationIdAsync(OrganizationId organizationId,
        CancellationToken cancellationToken) =>
        Context.Set<Organization>().FirstOrDefaultAsync(organization => organization.Id == organizationId,
            cancellationToken);
}
