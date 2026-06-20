namespace Powertech.Platform.Notifications.Interfaces.Rest.Resources;

public record CreateNotificationResource(Guid OrganizationId, Guid ParentId, Guid TripId, string Category, string Message);
