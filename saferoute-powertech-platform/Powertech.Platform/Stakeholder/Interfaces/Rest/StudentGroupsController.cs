using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Powertech.Platform.Stakeholder.Application.CommandServices;
using Powertech.Platform.Stakeholder.Application.QueryServices;
using Powertech.Platform.Stakeholder.Domain.Model;
using Powertech.Platform.Stakeholder.Domain.Model.Commands;
using Powertech.Platform.Stakeholder.Domain.Model.Queries;
using Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;
using Powertech.Platform.Stakeholder.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Powertech.Platform.Stakeholder.Interfaces.Rest;

[ApiController]
[Route("api/v1/student-groups")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Stakeholder student group endpoints.")]
public class StudentGroupsController(
    IStakeholderCommandService commandService,
    IStakeholderQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup(CreateStudentGroupResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new CreateStudentGroupCommand(resource.OrganizationId, resource.Name),
            cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            group => CreatedAtAction(nameof(GetGroupById), new { studentGroupId = group.Id.Identifier },
                StakeholderResourceFromEntityAssembler.ToResourceFromEntity(group)));
    }

    [HttpGet("{studentGroupId:guid}")]
    public async Task<IActionResult> GetGroupById(Guid studentGroupId, CancellationToken cancellationToken)
    {
        var group = await queryService.Handle(new GetStudentGroupByIdQuery(studentGroupId), cancellationToken);
        if (group is null)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                StakeholderError.StudentGroupNotFound, "Student group was not found.");
        return Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(group));
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
    {
        var groups = await queryService.Handle(new GetAllStudentGroupsQuery(), cancellationToken);
        return Ok(groups.Select(StakeholderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost("{studentGroupId:guid}/children")]
    public async Task<IActionResult> AddChild(Guid studentGroupId, GroupChildResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new AddChildToGroupCommand(studentGroupId, resource.ChildId),
            cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            group => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(group)));
    }

    [HttpDelete("{studentGroupId:guid}/children/{childId:guid}")]
    public async Task<IActionResult> RemoveChild(Guid studentGroupId, Guid childId,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new RemoveChildFromGroupCommand(studentGroupId, childId),
            cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            group => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(group)));
    }

    [HttpPost("{studentGroupId:guid}/finalize")]
    public async Task<IActionResult> FinalizeGroup(Guid studentGroupId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new FinalizeStudentGroupCommand(studentGroupId), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            group => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(group)));
    }
}
