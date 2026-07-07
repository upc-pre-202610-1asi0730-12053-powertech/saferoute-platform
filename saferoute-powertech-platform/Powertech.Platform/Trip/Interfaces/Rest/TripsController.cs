using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.ValueObjects;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Powertech.Platform.Trip.Application.CommandServices;
using Powertech.Platform.Trip.Application.QueryServices;
using Powertech.Platform.Trip.Domain.Model;
using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Domain.Model.Queries;
using Powertech.Platform.Trip.Interfaces.Rest.Resources;
using Powertech.Platform.Trip.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Interfaces.Rest;

/// <summary>
///     REST endpoints for the Trip Execution &amp; Monitoring bounded context.
/// </summary>
/// <remarks>
///     The controller is a thin adapter: it converts resources to commands/queries, delegates to the
///     application services, and translates the results to HTTP responses through the assemblers. It
///     contains no business logic.
/// </remarks>
/// <param name="tripCommandService">The trip command (write) service.</param>
/// <param name="tripQueryService">The trip query (read) service.</param>
/// <param name="errorLocalizer">The error message localizer.</param>
/// <param name="problemDetailsFactory">The factory used to produce RFC 7807 problem details.</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Trip execution and monitoring endpoints.")]
public class TripsController(
    ITripCommandService tripCommandService,
    ITripQueryService tripQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    AppDbContext context,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    /// <summary>Prepares (creates) a new trip session.</summary>
    [HttpPost]
    [SwaggerOperation("Create Trip", "Prepares a new trip session for a route and driver.",
        OperationId = "CreateTrip")]
    [SwaggerResponse(201, "The trip was created.", typeof(TripResource))]
    [SwaggerResponse(400, "The trip could not be created.")]
    public async Task<IActionResult> CreateTrip(CreateTripResource resource, CancellationToken cancellationToken)
    {
        var command = CreateTripCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await tripCommandService.Handle(command, cancellationToken);

        return TripActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            trip => CreatedAtAction(nameof(GetTripById), new { tripId = trip.Id.Identifier },
                TripResourceFromEntityAssembler.ToResourceFromEntity(trip)));
    }
    
    /// <summary>Starts a prepared trip.</summary>
    [HttpPost("{tripId:guid}/start")]
    [SwaggerOperation("Start Trip", "Starts a prepared trip.", OperationId = "StartTrip")]
    [SwaggerResponse(200, "The trip was started.", typeof(TripResource))]
    [SwaggerResponse(404, "The trip was not found.")]
    [SwaggerResponse(409, "The trip is not in a startable state.")]
    public async Task<IActionResult> StartTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var result = await tripCommandService.Handle(new StartTripCommand(tripId), cancellationToken);
        return TripActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            trip => Ok(TripResourceFromEntityAssembler.ToResourceFromEntity(trip)));
    }
    
    /// <summary>Gets a trip by its unique identifier.</summary>
    [HttpGet("{tripId:guid}")]
    [SwaggerOperation("Get Trip by Id", "Gets a trip by its unique identifier.", OperationId = "GetTripById")]
    [SwaggerResponse(200, "The trip was found.", typeof(TripResource))]
    [SwaggerResponse(404, "The trip was not found.")]
    public async Task<IActionResult> GetTripById(Guid tripId, CancellationToken cancellationToken)
    {
        var trip = await tripQueryService.Handle(new GetTripByIdQuery(tripId), cancellationToken);

        return TripActionResultAssembler.ToActionResultFromLookup(this, trip, errorLocalizer, problemDetailsFactory,
            found => Ok(TripResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }
    
    /// <summary>Gets all trips, optionally filtered by route.</summary>
    [HttpGet]
    [SwaggerOperation("Get Trips", "Gets all trips, optionally filtered by route id.", OperationId = "GetTrips")]
    [SwaggerResponse(200, "The trips were found.", typeof(IEnumerable<TripResource>))]
    public async Task<IActionResult> GetTrips([FromQuery] Guid? routeId, CancellationToken cancellationToken)
    {
        var trips = routeId is null
            ? await tripQueryService.Handle(new GetAllTripsQuery(), cancellationToken)
            : await tripQueryService.Handle(new GetTripsByRouteIdQuery(routeId.Value), cancellationToken);

        return Ok(trips.Select(TripResourceFromEntityAssembler.ToResourceFromEntity));
    }
    
    /// <summary>Records a child's boarding status during a trip.</summary>
    [HttpPost("{tripId:guid}/boarding")]
    [SwaggerOperation("Record Boarding", "Records a child's boarding status during a trip.",
        OperationId = "RecordBoarding")]
    [SwaggerResponse(200, "The boarding status was recorded.", typeof(TripResource))]
    [SwaggerResponse(404, "The trip was not found.")]
    [SwaggerResponse(409, "The trip is not in progress.")]
    public async Task<IActionResult> RecordBoarding(Guid tripId, SetBoardingStatusResource resource,
        CancellationToken cancellationToken)
    {
        var command = SetBoardingStatusCommandFromResourceAssembler.ToCommandFromResource(tripId, resource);
        var result = await tripCommandService.Handle(command, cancellationToken);
        return TripActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            trip => Ok(TripResourceFromEntityAssembler.ToResourceFromEntity(trip)));
    }

    /// <summary>Reports an incident during a trip.</summary>
    [HttpPost("{tripId:guid}/incidents")]
    [SwaggerOperation("Report Incident", "Reports an incident during a trip.", OperationId = "ReportIncident")]
    [SwaggerResponse(200, "The incident was reported.", typeof(TripResource))]
    [SwaggerResponse(404, "The trip was not found.")]
    [SwaggerResponse(409, "The trip is not in progress.")]
    public async Task<IActionResult> ReportIncident(Guid tripId, ReportIncidentResource resource,
        CancellationToken cancellationToken)
    {
        var command = ReportIncidentCommandFromResourceAssembler.ToCommandFromResource(tripId, resource);
        var result = await tripCommandService.Handle(command, cancellationToken);
        return TripActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            trip => Ok(TripResourceFromEntityAssembler.ToResourceFromEntity(trip)));
    }    
    
    /// <summary>Completes an in-progress trip.</summary>
    [HttpPost("{tripId:guid}/complete")]
    [SwaggerOperation("Complete Trip", "Completes an in-progress trip.", OperationId = "CompleteTrip")]
    [SwaggerResponse(200, "The trip was completed.", typeof(TripResource))]
    [SwaggerResponse(404, "The trip was not found.")]
    [SwaggerResponse(409, "The trip is not in progress.")]
    public async Task<IActionResult> CompleteTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var result = await tripCommandService.Handle(new CompleteTripCommand(tripId), cancellationToken);
        return TripActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            trip => Ok(TripResourceFromEntityAssembler.ToResourceFromEntity(trip)));
    }

    /// <summary>Deletes a trip and notification records associated with it.</summary>
    [HttpDelete("{tripId:guid}")]
    [SwaggerOperation("Delete Trip", "Deletes a trip and its notification records.", OperationId = "DeleteTrip")]
    [SwaggerResponse(204, "The trip was deleted.")]
    [SwaggerResponse(404, "The trip was not found.")]
    public async Task<IActionResult> DeleteTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var tripValue = new TripId(tripId);
        var trip = await context.Set<TripAggregate>()
            .FirstOrDefaultAsync(item => item.Id == tripValue, cancellationToken);

        if (trip is null)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                TripError.TripNotFound, errorLocalizer[nameof(TripError.TripNotFound)]);

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var notificationTripId = new NotificationId(tripId);
        var notifications = await context.Set<Notification>()
            .Where(notification => notification.TripId == notificationTripId)
            .ToListAsync(cancellationToken);

        context.RemoveRange(notifications);
        context.Remove(trip);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }
}
