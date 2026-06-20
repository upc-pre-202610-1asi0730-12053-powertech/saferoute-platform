using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Fleet.Domain.Model;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Interfaces.Rest.Transform;

/// <summary>
///     Translates Fleet application results into HTTP action results, mapping typed
///     <see cref="FleetError" /> values to status codes and RFC 7807 problem details.
/// </summary>
public static class FleetActionResultAssembler
{
    /// <summary>Maps a <see cref="FleetError" /> to its HTTP status code.</summary>
    private static int ToStatusCode(FleetError error) => error switch
    {
        FleetError.RouteNotFound => StatusCodes.Status404NotFound,
        FleetError.InvalidRouteState => StatusCodes.Status409Conflict,
        FleetError.InvalidRouteData => StatusCodes.Status400BadRequest,
        FleetError.OperationCancelled => StatusCodes.Status409Conflict,
        FleetError.DatabaseError => StatusCodes.Status500InternalServerError,
        FleetError.InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };

    /// <summary>
    ///     Produces an action result from a command <see cref="Result{T}" />: the success action on
    ///     success, or a problem-details response on failure.
    /// </summary>
    public static IActionResult ToActionResult(
        ControllerBase controller,
        Result<Route> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Route, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCode((FleetError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    /// <summary>
    ///     Produces an action result from a nullable query lookup: the success action when the route
    ///     exists, or a 404 problem-details response when it does not.
    /// </summary>
    public static IActionResult ToActionResultFromLookup(
        ControllerBase controller,
        Route? route,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Route, IActionResult> successAction)
    {
        if (route is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCode(FleetError.RouteNotFound),
                FleetError.RouteNotFound,
                errorLocalizer[nameof(FleetError.RouteNotFound)]);

        return successAction(route);
    }
}