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
[SwaggerTag("Stakeholder driver endpoints.")]
public class DriversController(
    IStakeholderCommandService commandService,
    IStakeholderQueryService queryService,
    IIamCommandService iamCommandService,
    AppDbContext context,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDriver(CreateDriverResource resource, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var userId = resource.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            if (string.IsNullOrWhiteSpace(resource.Password))
                return CreateIamProblem(StatusCodes.Status400BadRequest, IamError.InvalidIamData,
                    "A password is required to create the driver sign-in account.");

            var userResult = await iamCommandService.Handle(new SignUpCommand(resource.FirstName, resource.LastName,
                resource.Email, resource.Password, RoleTier.Driver, resource.OrganizationId), cancellationToken);

            if (userResult.IsFailure)
                return CreateIamProblem(ToIamStatusCode((IamError)userResult.Error!), (IamError)userResult.Error!,
                    userResult.Message);

            userId = userResult.Value!.Id.Identifier;
        }

        var command = new CreateDriverCommand(resource.OrganizationId, userId.Value, resource.FirstName,
            resource.LastName, resource.Email, resource.PhoneNumber, resource.LicenseNumber);
        var result = await commandService.Handle(command, cancellationToken);

        if (result.IsFailure)
            return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
                driver => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver)));

        await transaction.CommitAsync(cancellationToken);

        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            driver => CreatedAtAction(nameof(GetDriverById), new { driverId = driver.Id.Identifier },
                StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver)));
    }

    [HttpGet("{driverId:guid}")]
    public async Task<IActionResult> GetDriverById(Guid driverId, CancellationToken cancellationToken)
    {
        var driver = await queryService.Handle(new GetDriverByIdQuery(driverId), cancellationToken);
        if (driver is null)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                StakeholderError.DriverNotFound, "Driver was not found.");
        return Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver));
    }

    [HttpGet]
    public async Task<IActionResult> GetDrivers(CancellationToken cancellationToken)
    {
        var drivers = await queryService.Handle(new GetAllDriversQuery(), cancellationToken);
        return Ok(drivers.Select(StakeholderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{driverId:guid}")]
    public async Task<IActionResult> UpdateDriver(Guid driverId, CreateDriverResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new UpdateDriverCommand(driverId, resource.FirstName,
            resource.LastName, resource.Email, resource.PhoneNumber, resource.LicenseNumber, true), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            driver => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver)));
    }

    [HttpDelete("{driverId:guid}")]
    public async Task<IActionResult> DeleteDriver(Guid driverId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new DeleteDriverCommand(driverId), cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            driver => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver)));
    }

    [HttpPut("{driverId:guid}/phone-number")]
    public async Task<IActionResult> UpdatePhone(Guid driverId, UpdateDriverPhoneResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new UpdateDriverPhoneCommand(driverId, resource.PhoneNumber),
            cancellationToken);
        return StakeholderActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            driver => Ok(StakeholderResourceFromEntityAssembler.ToResourceFromEntity(driver)));
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
