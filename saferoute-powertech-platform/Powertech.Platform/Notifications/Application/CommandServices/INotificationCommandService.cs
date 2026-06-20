using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Shared.Application.Model;

namespace Powertech.Platform.Notifications.Application.CommandServices;

public interface INotificationCommandService
{
    Task<Result<Notification>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default);
    Task<Result<Notification>> Handle(DispatchNotificationCommand command, CancellationToken cancellationToken = default);
    Task<Result<Notification>> Handle(MarkNotificationDeliveredCommand command, CancellationToken cancellationToken = default);
}
