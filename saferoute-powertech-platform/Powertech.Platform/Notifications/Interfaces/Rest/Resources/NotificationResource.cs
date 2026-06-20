namespace Powertech.Platform.Notifications.Interfaces.Rest.Resources;

public record NotificationResource(
    Guid Id,
    Guid OrganizationId,
    Guid ParentId,
    Guid TripId,
    string Category,
    string DeliveryState,
    string Message,
    DateTimeOffset SentAt,
    IEnumerable<AlertResource> Alerts,
    IEnumerable<AnnouncementResource> Announcements);
