using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;

namespace Powertech.Platform.Iam.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
    Task<Organization?> FindByOrganizationIdAsync(OrganizationId organizationId, CancellationToken cancellationToken);
}
