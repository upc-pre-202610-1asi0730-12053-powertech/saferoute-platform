namespace Powertech.Platform.Notifications.Domain.Model.ValueObjects;

public record NotificationId(Guid Identifier)
{
    public NotificationId() : this(Guid.NewGuid())
    {
    }
}
