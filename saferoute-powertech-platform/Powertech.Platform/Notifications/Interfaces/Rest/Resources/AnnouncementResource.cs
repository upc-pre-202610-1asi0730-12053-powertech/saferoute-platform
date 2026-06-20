namespace Powertech.Platform.Notifications.Interfaces.Rest.Resources;

public record AnnouncementResource(Guid Id, Guid RouteId, string Message, DateTimeOffset PublishedAt);
