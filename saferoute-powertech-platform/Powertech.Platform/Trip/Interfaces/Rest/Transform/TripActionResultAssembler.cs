using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;
using Powertech.Platform.Trip.Domain.Model;

namespace Powertech.Platform.Trip.Interfaces.Rest.Transform;

/// <summary>
///     Translates Trip application results into HTTP action results, mapping typed
///     <see cref="TripError" /> values to the appropriate status codes and RFC 7807 problem details.
/// </summary>
public static class TripActionResultAssembler
{
    /// <summary>Maps a <see cref="TripError" /> to its HTTP status code.</summary>
    private static int ToStatusCode(TripError error) => error switch
    {
        TripError.TripNotFound => StatusCodes.Status404NotFound,
        TripError.InvalidTripState => StatusCodes.Status409Conflict,
        TripError.InvalidTripData => StatusCodes.Status400BadRequest,
        TripError.OperationCancelled => StatusCodes.Status409Conflict,
        TripError.DatabaseError => StatusCodes.Status500InternalServerError,
        TripError.InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };

    /// <summary>
    ///     Produces an action result from a command <see cref="Result{T}" />: the success action on
    ///     success, or a problem-details response carrying the mapped status code on failure.
    /// </summary>
    /// <param name="controller">The calling controller.</param>
    /// <param name="result">The command result.</param>
    /// <param name="problemDetailsFactory">The problem-details factory.</param>
    /// <param name="successAction">The action to run when the result is successful.</param>
    public static IActionResult ToActionResult(
        ControllerBase controller,
        Result<TripAggregate> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<TripAggregate, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCode((TripError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    /// <summary>
    ///     Produces an action result from a nullable query lookup: the success action when the trip
    ///     exists, or a 404 problem-details response when it does not.
    /// </summary>
    /// <param name="controller">The calling controller.</param>
    /// <param name="trip">The trip looked up, or <c>null</c>.</param>
    /// <param name="errorLocalizer">The error message localizer.</param>
    /// <param name="problemDetailsFactory">The problem-details factory.</param>
    /// <param name="successAction">The action to run when the trip exists.</param>
    public static IActionResult ToActionResultFromLookup(
        ControllerBase controller,
        TripAggregate? trip,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<TripAggregate, IActionResult> successAction)
    {
        if (trip is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCode(TripError.TripNotFound),
                TripError.TripNotFound,
                errorLocalizer[nameof(TripError.TripNotFound)]);

        return successAction(trip);
    }
}