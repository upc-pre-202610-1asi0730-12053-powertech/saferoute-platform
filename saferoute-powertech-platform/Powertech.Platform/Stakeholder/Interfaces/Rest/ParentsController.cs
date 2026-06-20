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
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Stakeholder parent endpoints.")]
public class ParentsController(
    IStakeholderCommandService commandService,
    IStakeholderQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateParent(CreateParentResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new CreateParentCommand(resource.OrganizationId, resource.UserId,
            resource.FirstName, resource.LastName, resource.Email, resource.PhoneNumber), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            parent => CreatedAtAction(nameof(GetParentById), new { parentId = parent.Id.Identifier },
                StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));
    }

    [HttpGet("{parentId:guid}")]
    public async Task<IActionResult> GetParentById(Guid parentId, CancellationToken cancellationToken)
    {
        var parent = await queryService.Handle(new GetParentByIdQuery(parentId), cancellationToken);
        if (parent is null)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                StakeholderError.ParentNotFound, "Parent was not found.");
        return Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent));
    }

    [HttpGet]
    public async Task<IActionResult> GetParents(CancellationToken cancellationToken)
    {
        var parents = await queryService.Handle(new GetAllParentsQuery(), cancellationToken);
        return Ok(parents.Select(StakeholderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{parentId:guid}")]
    public async Task<IActionResult> UpdateParent(Guid parentId, CreateParentResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new UpdateParentCommand(parentId, resource.FirstName,
            resource.LastName, resource.Email, resource.PhoneNumber), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            parent => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));
    }

    [HttpDelete("{parentId:guid}")]
    public async Task<IActionResult> DeleteParent(Guid parentId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new DeleteParentCommand(parentId), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            parent => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));
    }

    [HttpPost("{parentId:guid}/children")]
    public async Task<IActionResult> AddChild(Guid parentId, AddChildResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new AddChildToParentCommand(parentId, resource.FirstName,
            resource.LastName, resource.Age), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            parent => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));
    }

    [HttpDelete("{parentId:guid}/children/{childId:guid}")]
    public async Task<IActionResult> RemoveChild(Guid parentId, Guid childId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new RemoveChildFromParentCommand(parentId, childId),
            cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            parent => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));
    }
}
