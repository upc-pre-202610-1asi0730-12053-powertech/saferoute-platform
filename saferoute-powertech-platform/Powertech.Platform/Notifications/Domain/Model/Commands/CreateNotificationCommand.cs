namespace Powertech.Platform.Notifications.Domain.Model.Commands;

public record CreateNotificationCommand(Guid OrganizationId, Guid ParentId, Guid TripId, string Category, string Message);