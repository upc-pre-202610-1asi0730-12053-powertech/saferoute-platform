namespace Powertech.Platform.Notifications.Domain.Model.Commands;

public record TriggerAlertCommand(Guid NotificationId, bool Panic);
