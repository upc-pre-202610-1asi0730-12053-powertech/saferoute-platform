namespace Powertech.Platform.Notifications.Interfaces.Rest.Resources;

public record AlertResource(Guid Id, DateTimeOffset TriggeredAt, bool Panic);
