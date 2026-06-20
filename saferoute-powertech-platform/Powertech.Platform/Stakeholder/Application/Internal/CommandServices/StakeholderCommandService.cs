using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Stakeholder.Application.CommandServices;
using Powertech.Platform.Stakeholder.Domain.Model;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Commands;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Repositories;

namespace Powertech.Platform.Stakeholder.Application.Internal.CommandServices;

public class StakeholderCommandService(
    IParentRepository parentRepository,
    IDriverRepository driverRepository,
    IStudentGroupRepository studentGroupRepository,
    IUnitOfWork unitOfWork) : IStakeholderCommandService
{
    public async Task<Result<Parent>> Handle(CreateParentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var parent = new Parent(command);
            await parentRepository.AddAsync(parent, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Parent>.Success(parent);
        }
        catch (ArgumentException ex)
        {
            return Result<Parent>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result<Parent>.Failure(StakeholderError.DatabaseError, "Database error.");
        }
    }

    public async Task<Result<Parent>> Handle(UpdateParentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var parent = await parentRepository.FindByParentIdAsync(new ParentId(command.ParentId), cancellationToken);
            if (parent is null)
                return Result<Parent>.Failure(StakeholderError.ParentNotFound, "Parent was not found.");
            parent.Update(new FullName(command.FirstName, command.LastName), new Email(command.Email),
                new PhoneNumber(command.PhoneNumber));
            parentRepository.Update(parent);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Parent>.Success(parent);
        }
        catch (ArgumentException ex)
        {
            return Result<Parent>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    public async Task<Result<Parent>> Handle(DeleteParentCommand command, CancellationToken cancellationToken)
    {
        var parent = await parentRepository.FindByParentIdAsync(new ParentId(command.ParentId), cancellationToken);
        if (parent is null)
            return Result<Parent>.Failure(StakeholderError.ParentNotFound, "Parent was not found.");
        parentRepository.Remove(parent);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Parent>.Success(parent);
    }

    public Task<Result<Parent>> Handle(AddChildToParentCommand command, CancellationToken cancellationToken) =>
        MutateParentAsync(command.ParentId,
            parent => parent.AddChild(new Child(new FullName(command.FirstName, command.LastName), command.Age)),
            cancellationToken);

    public Task<Result<Parent>> Handle(RemoveChildFromParentCommand command, CancellationToken cancellationToken) =>
        MutateParentAsync(command.ParentId, parent => parent.RemoveChild(new ChildId(command.ChildId)),
            cancellationToken);

    public async Task<Result<Driver>> Handle(CreateDriverCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var driver = new Driver(command);
            await driverRepository.AddAsync(driver, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Driver>.Success(driver);
        }
        catch (ArgumentException ex)
        {
            return Result<Driver>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    public async Task<Result<Driver>> Handle(UpdateDriverCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var driver = await driverRepository.FindByDriverIdAsync(new DriverId(command.DriverId), cancellationToken);
            if (driver is null)
                return Result<Driver>.Failure(StakeholderError.DriverNotFound, "Driver was not found.");
            driver.Update(new FullName(command.FirstName, command.LastName), new Email(command.Email),
                new PhoneNumber(command.PhoneNumber), new LicenseNumber(command.LicenseNumber), command.Available);
            driverRepository.Update(driver);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Driver>.Success(driver);
        }
        catch (ArgumentException ex)
        {
            return Result<Driver>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    public async Task<Result<Driver>> Handle(DeleteDriverCommand command, CancellationToken cancellationToken)
    {
        var driver = await driverRepository.FindByDriverIdAsync(new DriverId(command.DriverId), cancellationToken);
        if (driver is null)
            return Result<Driver>.Failure(StakeholderError.DriverNotFound, "Driver was not found.");
        driverRepository.Remove(driver);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Driver>.Success(driver);
    }

    public async Task<Result<Driver>> Handle(UpdateDriverPhoneCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var driver = await driverRepository.FindByDriverIdAsync(new DriverId(command.DriverId), cancellationToken);
            if (driver is null)
                return Result<Driver>.Failure(StakeholderError.DriverNotFound, "Driver was not found.");
            driver.UpdatePhoneNumber(new PhoneNumber(command.PhoneNumber));
            driverRepository.Update(driver);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Driver>.Success(driver);
        }
        catch (ArgumentException ex)
        {
            return Result<Driver>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    public async Task<Result<StudentGroup>> Handle(CreateStudentGroupCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = new StudentGroup(new OrganizationId(command.OrganizationId), command.Name);
            await studentGroupRepository.AddAsync(group, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<StudentGroup>.Success(group);
        }
        catch (ArgumentException ex)
        {
            return Result<StudentGroup>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    public Task<Result<StudentGroup>> Handle(AddChildToGroupCommand command, CancellationToken cancellationToken) =>
        MutateGroupAsync(command.StudentGroupId, group => group.AddChild(new ChildId(command.ChildId)),
            cancellationToken);

    public Task<Result<StudentGroup>> Handle(RemoveChildFromGroupCommand command, CancellationToken cancellationToken) =>
        MutateGroupAsync(command.StudentGroupId, group => group.RemoveChild(new ChildId(command.ChildId)),
            cancellationToken);

    public Task<Result<StudentGroup>> Handle(FinalizeStudentGroupCommand command, CancellationToken cancellationToken) =>
        MutateGroupAsync(command.StudentGroupId, group => group.Finalize(), cancellationToken);

    private async Task<Result<Parent>> MutateParentAsync(Guid parentId, Action<Parent> mutation,
        CancellationToken cancellationToken)
    {
        try
        {
            var parent = await parentRepository.FindByParentIdAsync(new ParentId(parentId), cancellationToken);
            if (parent is null)
                return Result<Parent>.Failure(StakeholderError.ParentNotFound, "Parent was not found.");
            mutation(parent);
            parentRepository.Update(parent);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Parent>.Success(parent);
        }
        catch (ArgumentException ex)
        {
            return Result<Parent>.Failure(StakeholderError.InvalidStakeholderData, ex.Message);
        }
    }

    private async Task<Result<StudentGroup>> MutateGroupAsync(Guid groupId, Action<StudentGroup> mutation,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await studentGroupRepository.FindByStudentGroupIdAsync(new StudentGroupId(groupId),
                cancellationToken);
            if (group is null)
                return Result<StudentGroup>.Failure(StakeholderError.StudentGroupNotFound,
                    "Student group was not found.");
            mutation(group);
            studentGroupRepository.Update(group);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<StudentGroup>.Success(group);
        }
        catch (InvalidOperationException ex)
        {
            return Result<StudentGroup>.Failure(StakeholderError.InvalidStudentGroupState, ex.Message);
        }
    }
}
