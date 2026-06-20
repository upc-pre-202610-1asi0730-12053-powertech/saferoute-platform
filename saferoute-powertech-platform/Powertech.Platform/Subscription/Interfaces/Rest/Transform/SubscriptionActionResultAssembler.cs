using Microsoft.AspNetCore.Mvc;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Powertech.Platform.Subscription.Domain.Model;

namespace Powertech.Platform.Subscription.Interfaces.Rest.Transform;

public static class SubscriptionActionResultAssembler
{
    private static int ToStatusCode(SubscriptionError error) => error switch
    {
        SubscriptionError.PlanNotFound or SubscriptionError.SubscriptionNotFound => StatusCodes.Status404NotFound,
        SubscriptionError.InvalidSubscriptionData => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    public static IActionResult ToActionResult<T>(
        ControllerBase controller,
        Result<T> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);
        return problemDetailsFactory.CreateProblemDetails(controller,
            ToStatusCode((SubscriptionError)result.Error!), result.Error, result.Message);
    }
}
