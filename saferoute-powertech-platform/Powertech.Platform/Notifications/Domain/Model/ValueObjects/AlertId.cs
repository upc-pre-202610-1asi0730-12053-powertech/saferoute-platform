namespace Powertech.Platform.Notifications.Domain.Model.ValueObjects;

public record AlertId(Guid Identifier)
{
    public AlertId() : this(Guid.NewGuid())
    {
    }
}
