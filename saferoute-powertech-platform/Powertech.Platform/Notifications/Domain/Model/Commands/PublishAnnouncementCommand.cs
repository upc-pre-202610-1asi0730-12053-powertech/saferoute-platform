namespace Powertech.Platform.Notifications.Domain.Model.Commands;

public record PublishAnnouncementCommand(Guid NotificationId, Guid RouteId, string Message);
