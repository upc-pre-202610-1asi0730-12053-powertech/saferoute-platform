using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Fleet.Application.CommandServices;
using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Fleet.Interfaces.Rest.Resources;
using Powertech.Platform.Fleet.Interfaces.Rest.Transform;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Powertech.Platform.Fleet.Interfaces.Rest;

/// <summary>
///     REST endpoints for the Fleet &amp; Route Planning bounded context.
/// </summary>
/// <remarks>
///     The controller is a thin adapter that converts resources into commands/queries, delegates to
///     the application services and translates results into HTTP responses through the assemblers.
///     It holds no business logic.
/// </remarks>
/// <param name="routeCommandService">The route command (write) service.</param>
/// <param name="routeQueryService">The route query (read) service.</param>
/// <param name="errorLocalizer">The error message localizer.</param>
/// <param name="problemDetailsFactory">The factory used to produce RFC 7807 problem details.</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Fleet and route planning endpoints.")]
public class RoutesController(
    IRouteCommandService routeCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    
    /// <summary>Assigns a child (student) to a route.</summary>
    [HttpPost("{routeId:guid}/children")]
    [SwaggerOperation("Assign Student", "Assigns a student to a route.", OperationId = "AssignStudent")]
    [SwaggerResponse(200, "The student was assigned.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> AssignStudent(Guid routeId, AssignStudentResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AssignStudentsToRouteCommand(routeId, Guid.Parse(resource.ChildId));
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Removes a child (student) from a route.</summary>
    [HttpDelete("{routeId:guid}/children/{childId:guid}")]
    [SwaggerOperation("Remove Student", "Removes a student from a route.", OperationId = "RemoveStudent")]
    [SwaggerResponse(200, "The student was removed.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> RemoveStudent(Guid routeId, Guid childId, CancellationToken cancellationToken)
    {
        var result = await routeCommandService.Handle(new RemoveStudentsFromRouteCommand(routeId, childId),
            cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }
}