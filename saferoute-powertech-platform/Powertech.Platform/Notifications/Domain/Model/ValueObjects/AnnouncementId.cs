namespace Powertech.Platform.Notifications.Domain.Model.ValueObjects;

public record AnnouncementId(Guid Identifier)
{
    public AnnouncementId() : this(Guid.NewGuid())
    {
    }
}
