using Powertech.Platform.Notifications.Domain.Model.ValueObjects;

namespace Powertech.Platform.Notifications.Domain.Model.Entities;

public class Announcement
{
    public AnnouncementId Id { get; private set; }
    public NotificationId NotificationId { get; private set; }
    public NotificationId RouteId { get; private set; }
    public NotificationMessage Message { get; private set; }
    public DateTimeOffset PublishedAt { get; private set; }

    protected Announcement()
    {
        Id = new AnnouncementId();
        NotificationId = new NotificationId(Guid.Empty);
        RouteId = new NotificationId(Guid.Empty);
        Message = new NotificationMessage(string.Empty);
    }

    public Announcement(NotificationId notificationId, Guid routeId, string message)
    {
        Id = new AnnouncementId();
        NotificationId = notificationId;
        RouteId = new NotificationId(routeId);
        Message = new NotificationMessage(message);
        PublishedAt = DateTimeOffset.UtcNow;
    }
}
