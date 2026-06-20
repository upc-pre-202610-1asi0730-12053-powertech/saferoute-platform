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
[SwaggerTag("Stakeholder driver endpoints.")]
public class DriversController(
    IStakeholderCommandService commandService,
    IStakeholderQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDriver(CreateDriverResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateDriverCommand(resource.OrganizationId, resource.UserId, resource.FirstName,
            resource.LastName, resource.Email, resource.PhoneNumber, resource.LicenseNumber);
        var result = await commandService.Handle(command, cancellationToken);
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
}
