using Powertech.Platform.Notifications.Application.CommandServices;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Shared.Application.Model;

namespace Powertech.Platform.Notifications.Application.Internal.CommandServices;

public class NotificationCommandService : INotificationCommandService
{
    public async Task<Result<Notification>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}