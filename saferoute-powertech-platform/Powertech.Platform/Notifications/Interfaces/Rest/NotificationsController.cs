using Microsoft.AspNetCore.Mvc;
using Powertech.Platform.Notifications.Application.CommandServices;
using Powertech.Platform.Notifications.Application.QueryServices;
using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Notifications.Domain.Model.Queries;
using Powertech.Platform.Notifications.Interfaces.Rest.Resources;
using Powertech.Platform.Notifications.Interfaces.Rest.Transform;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Powertech.Platform.Notifications.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController(
    INotificationCommandService commandService,
    INotificationQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateNotificationCommand(resource.OrganizationId, resource.ParentId, resource.TripId, resource.Category, resource.Message);
        var result = await commandService.Handle(command, cancellationToken);
        
        return NotificationActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            notification => CreatedAtAction(nameof(GetNotificationById), new { notificationId = notification.Id.Identifier }, 
                NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification)));
    }

    [HttpGet("{notificationId:guid}")]
    public async Task<IActionResult> GetNotificationById(Guid notificationId, CancellationToken cancellationToken)
    {
        var notification = await queryService.Handle(new GetNotificationByIdQuery(notificationId), cancellationToken);
        if (notification is null) return NotFound();
        return Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification));
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] Guid? parentId, CancellationToken cancellationToken)
    {
        IEnumerable<Powertech.Platform.Notifications.Domain.Model.Aggregates.Notification> notifications;
        if (parentId.HasValue)
        {
            notifications = await queryService.Handle(new GetNotificationsByParentIdQuery(parentId.Value), cancellationToken);
        }
        else
        {
            notifications = await queryService.Handle(new GetAllNotificationsQuery(), cancellationToken);
        }
        
        var resources = notifications.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost("{notificationId:guid}/dispatch")]
    public async Task<IActionResult> Dispatch(Guid notificationId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new DispatchNotificationCommand(notificationId), cancellationToken);
        return NotificationActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            notification => Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification)));
    }

    [HttpPost("{notificationId:guid}/delivered")]
    public async Task<IActionResult> MarkDelivered(Guid notificationId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new MarkNotificationDeliveredCommand(notificationId), cancellationToken);
        return NotificationActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            notification => Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification)));
    }

    [HttpPost("{notificationId:guid}/alerts")]
    public async Task<IActionResult> TriggerAlert(Guid notificationId, TriggerAlertResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new TriggerAlertCommand(notificationId, resource.Panic), cancellationToken);
        return NotificationActionResultAssembler.ToActionResult(this, result, problemDetailsFactory,
            notification => Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification)));
    }
}
