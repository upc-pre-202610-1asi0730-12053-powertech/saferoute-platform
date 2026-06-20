using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Repositories;

namespace Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class StudentGroupRepository(AppDbContext context)
    : BaseRepository<StudentGroup>(context), IStudentGroupRepository
{
    public Task<StudentGroup?> FindByStudentGroupIdAsync(StudentGroupId studentGroupId,
        CancellationToken cancellationToken) =>
        Context.Set<StudentGroup>().FirstOrDefaultAsync(group => group.Id == studentGroupId, cancellationToken);
}
