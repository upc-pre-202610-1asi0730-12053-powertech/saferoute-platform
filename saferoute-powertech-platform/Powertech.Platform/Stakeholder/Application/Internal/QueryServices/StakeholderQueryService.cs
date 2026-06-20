using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Application.QueryServices;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.Queries;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Repositories;

namespace Powertech.Platform.Stakeholder.Application.Internal.QueryServices;

public class StakeholderQueryService(
    IParentRepository parentRepository,
    IDriverRepository driverRepository,
    IStudentGroupRepository studentGroupRepository) : IStakeholderQueryService
{
    public Task<Parent?> Handle(GetParentByIdQuery query, CancellationToken cancellationToken) =>
        parentRepository.FindByParentIdAsync(new ParentId(query.ParentId), cancellationToken);

    public async Task<IEnumerable<Parent>> Handle(GetAllParentsQuery query, CancellationToken cancellationToken) =>
        await parentRepository.ListAsync(cancellationToken);

    public Task<Driver?> Handle(GetDriverByIdQuery query, CancellationToken cancellationToken) =>
        driverRepository.FindByDriverIdAsync(new DriverId(query.DriverId), cancellationToken);

    public async Task<IEnumerable<Driver>> Handle(GetAllDriversQuery query, CancellationToken cancellationToken) =>
        await driverRepository.ListAsync(cancellationToken);

    public Task<StudentGroup?> Handle(GetStudentGroupByIdQuery query, CancellationToken cancellationToken) =>
        studentGroupRepository.FindByStudentGroupIdAsync(new StudentGroupId(query.StudentGroupId), cancellationToken);

    public async Task<IEnumerable<StudentGroup>> Handle(GetAllStudentGroupsQuery query,
        CancellationToken cancellationToken) =>
        await studentGroupRepository.ListAsync(cancellationToken);
}
