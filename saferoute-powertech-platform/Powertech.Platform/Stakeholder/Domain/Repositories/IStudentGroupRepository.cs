using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Domain.Repositories;

public interface IStudentGroupRepository : IBaseRepository<StudentGroup>
{
    Task<StudentGroup?> FindByStudentGroupIdAsync(StudentGroupId studentGroupId, CancellationToken cancellationToken);
}
