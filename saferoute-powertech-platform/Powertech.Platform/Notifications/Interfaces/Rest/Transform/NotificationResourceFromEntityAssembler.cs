using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Entities;
using Powertech.Platform.Notifications.Interfaces.Rest.Resources;

namespace Powertech.Platform.Notifications.Interfaces.Rest.Transform;

public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification notification) =>
        new(notification.Id.Identifier, notification.OrganizationId.Identifier, notification.ParentId.Identifier,
            notification.TripId.Identifier, notification.Category.Value, notification.DeliveryState.Value,
            notification.Message.Content, notification.SentAt, 
            Array.Empty<AlertResource>(), Array.Empty<AnnouncementResource>());
}
