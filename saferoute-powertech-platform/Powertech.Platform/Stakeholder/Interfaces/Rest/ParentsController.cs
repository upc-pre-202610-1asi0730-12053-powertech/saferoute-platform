using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Powertech.Platform.Iam.Application.CommandServices;
using Powertech.Platform.Iam.Domain.Model;
using Powertech.Platform.Iam.Domain.Model.Commands;
using Powertech.Platform.Iam.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
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
    IIamCommandService iamCommandService,
    AppDbContext context,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateParent(CreateParentResource resource, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var userId = resource.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            if (string.IsNullOrWhiteSpace(resource.Password))
                return CreateIamProblem(StatusCodes.Status400BadRequest, IamError.InvalidIamData,
                    "A password is required to create the parent sign-in account.");

            var userResult = await iamCommandService.Handle(new SignUpCommand(resource.FirstName, resource.LastName,
                resource.Email, resource.Password, RoleTier.Parent, resource.OrganizationId), cancellationToken);

            if (userResult.IsFailure)
                return CreateIamProblem(ToIamStatusCode((IamError)userResult.Error!), (IamError)userResult.Error!,
                    userResult.Message);

            userId = userResult.Value!.Id.Identifier;
        }

        var result = await commandService.Handle(new CreateParentCommand(resource.OrganizationId, userId.Value,
            resource.FirstName, resource.LastName, resource.Email, resource.PhoneNumber), cancellationToken);

        if (result.IsFailure)
            return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
                parent => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(parent)));

        await transaction.CommitAsync(cancellationToken);

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

    private IActionResult CreateIamProblem(int statusCode, IamError error, string detail) =>
        problemDetailsFactory.CreateProblemDetails(this, statusCode, error, detail);

    private static int ToIamStatusCode(IamError error) => error switch
    {
        IamError.UserNotFound or IamError.OrganizationNotFound => StatusCodes.Status404NotFound,
        IamError.EmailAlreadyRegistered => StatusCodes.Status409Conflict,
        IamError.InvalidCredentials or IamError.InvalidIamData => StatusCodes.Status400BadRequest,
        IamError.OperationCancelled => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
