using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Fleet.Application.QueryServices;
using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Fleet.Domain.Model.Queries;
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
    IRouteQueryService routeQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    /// <summary>Defines (creates) a new route in the draft state.</summary>
    [HttpPost]
    [SwaggerOperation("Create Route", "Defines a new route in the draft state.", OperationId = "CreateRoute")]
    [SwaggerResponse(201, "The route was created.", typeof(RouteResource))]
    [SwaggerResponse(400, "The route could not be created.")]
    public async Task<IActionResult> CreateRoute(CreateRouteResource resource, CancellationToken cancellationToken)
    {
        var command = CreateRouteCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await routeCommandService.Handle(command, cancellationToken);

        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => CreatedAtAction(nameof(GetRouteById), new { routeId = route.Id.Identifier },
                RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Gets a route by its unique identifier.</summary>
    [HttpGet("{routeId:guid}")]
    [SwaggerOperation("Get Route by Id", "Gets a route by its unique identifier.", OperationId = "GetRouteById")]
    [SwaggerResponse(200, "The route was found.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    public async Task<IActionResult> GetRouteById(Guid routeId, CancellationToken cancellationToken)
    {
        var route = await routeQueryService.Handle(new GetRouteByIdQuery(routeId), cancellationToken);
        return FleetActionResultAssembler.ToActionResultFromLookup(this, route, errorLocalizer, problemDetailsFactory,
            found => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>Gets all routes, optionally filtered by organization.</summary>
    [HttpGet]
    [SwaggerOperation("Get Routes", "Gets all routes, optionally filtered by organization id.",
        OperationId = "GetRoutes")]
    [SwaggerResponse(200, "The routes were found.", typeof(IEnumerable<RouteResource>))]
    public async Task<IActionResult> GetRoutes([FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var routes = organizationId is null
            ? await routeQueryService.Handle(new GetAllRoutesQuery(), cancellationToken)
            : await routeQueryService.Handle(new GetRoutesByOrganizationIdQuery(organizationId.Value),
                cancellationToken);

        return Ok(routes.Select(RouteResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Appends a stop to a route.</summary>
    [HttpPost("{routeId:guid}/stops")]
    [SwaggerOperation("Add Stop", "Appends a stop to a route.", OperationId = "AddStop")]
    [SwaggerResponse(200, "The stop was added.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> AddStop(Guid routeId, AddStopResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AddStopCommand(routeId, resource.Name, resource.Latitude, resource.Longitude);
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Removes a stop from a route.</summary>
    [HttpDelete("{routeId:guid}/stops/{stopId:guid}")]
    [SwaggerOperation("Remove Stop", "Removes a stop from a route.", OperationId = "RemoveStop")]
    [SwaggerResponse(200, "The stop was removed.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> RemoveStop(Guid routeId, Guid stopId, CancellationToken cancellationToken)
    {
        var result = await routeCommandService.Handle(new RemoveStopCommand(routeId, stopId), cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Assigns a vehicle to a route.</summary>
    [HttpPut("{routeId:guid}/vehicle")]
    [SwaggerOperation("Assign Vehicle", "Assigns a vehicle to a route.", OperationId = "AssignVehicle")]
    [SwaggerResponse(200, "The vehicle was assigned.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> AssignVehicle(Guid routeId, AssignVehicleResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AssignVehicleCommand(routeId, resource.Plate, resource.Model, resource.Brand,
            resource.Capacity);
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Assigns the operating driver to a route.</summary>
    [HttpPut("{routeId:guid}/driver")]
    [SwaggerOperation("Assign Driver", "Assigns the operating driver to a route.", OperationId = "AssignDriver")]
    [SwaggerResponse(200, "The driver was assigned.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> AssignDriver(Guid routeId, AssignDriverResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AssignDriverCommand(routeId, Guid.Parse(resource.DriverId));
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Assigns a child (student) to a route.</summary>
    [HttpPost("{routeId:guid}/children")]
    [SwaggerOperation("Assign Child", "Assigns a child to a route.", OperationId = "AssignChild")]
    [SwaggerResponse(200, "The child was assigned.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> AssignChild(Guid routeId, AssignStudentsResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AssignStudentsToRouteCommand(routeId, Guid.Parse(resource.ChildId));
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Removes a child (student) from a route.</summary>
    [HttpDelete("{routeId:guid}/children/{childId:guid}")]
    [SwaggerOperation("Remove Child", "Removes a child from a route.", OperationId = "RemoveChild")]
    [SwaggerResponse(200, "The child was removed.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> RemoveChild(Guid routeId, Guid childId, CancellationToken cancellationToken)
    {
        var result = await routeCommandService.Handle(new RemoveStudentsFromRouteCommand(routeId, childId),
            cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Defines the service days of a route.</summary>
    [HttpPut("{routeId:guid}/service-days")]
    [SwaggerOperation("Define Service Days", "Defines the weekdays on which a route operates.",
        OperationId = "DefineServiceDays")]
    [SwaggerResponse(200, "The service days were defined.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> DefineServiceDays(Guid routeId, DefineServiceDaysResource resource,
        CancellationToken cancellationToken)
    {
        var command = new DefineServiceDaysCommand(routeId, resource.Days);
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Sets the departure time of a route.</summary>
    [HttpPut("{routeId:guid}/departure-time")]
    [SwaggerOperation("Set Departure Time", "Sets the departure time of a route.",
        OperationId = "SetDepartureTime")]
    [SwaggerResponse(200, "The departure time was set.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not editable.")]
    public async Task<IActionResult> SetDepartureTime(Guid routeId, SetDepartureTimeResource resource,
        CancellationToken cancellationToken)
    {
        var command = new SetDepartureTimeCommand(routeId, resource.DepartureTime);
        var result = await routeCommandService.Handle(command, cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Finalizes the setup and activates a route.</summary>
    [HttpPost("{routeId:guid}/activate")]
    [SwaggerOperation("Activate Route", "Finalizes the setup and activates a route.", OperationId = "ActivateRoute")]
    [SwaggerResponse(200, "The route was activated.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route setup is incomplete or it is not a draft.")]
    public async Task<IActionResult> ActivateRoute(Guid routeId, CancellationToken cancellationToken)
    {
        var result = await routeCommandService.Handle(new ActivateRouteCommand(routeId), cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }

    /// <summary>Deactivates an active route.</summary>
    [HttpPost("{routeId:guid}/deactivate")]
    [SwaggerOperation("Deactivate Route", "Deactivates an active route.", OperationId = "DeactivateRoute")]
    [SwaggerResponse(200, "The route was deactivated.", typeof(RouteResource))]
    [SwaggerResponse(404, "The route was not found.")]
    [SwaggerResponse(409, "The route is not active.")]
    public async Task<IActionResult> DeactivateRoute(Guid routeId, CancellationToken cancellationToken)
    {
        var result = await routeCommandService.Handle(new DeactivateRouteCommand(routeId), cancellationToken);
        return FleetActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            route => Ok(RouteResourceFromEntityAssembler.ToResourceFromEntity(route)));
    }
}
