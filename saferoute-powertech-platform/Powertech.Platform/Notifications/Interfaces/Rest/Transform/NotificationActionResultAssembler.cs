using Microsoft.AspNetCore.Mvc;
using Powertech.Platform.Notifications.Domain.Model;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Powertech.Platform.Notifications.Interfaces.Rest.Transform;

public static class NotificationActionResultAssembler
{
    private static int ToStatusCode(NotificationError error) => error switch
    {
        NotificationError.NotificationNotFound => StatusCodes.Status404NotFound,
        NotificationError.InvalidNotificationData => StatusCodes.Status400BadRequest,
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
            ToStatusCode((NotificationError)result.Error!), result.Error, result.Message);
    }
}