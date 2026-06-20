using Powertech.Platform.Notifications.Application.QueryServices;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Queries;
using Powertech.Platform.Notifications.Domain.Repositories;

namespace Powertech.Platform.Notifications.Application.Internal.QueryServices;

public class NotificationQueryService(INotificationRepository notificationRepository) : INotificationQueryService
{
    public async Task<Notification?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await notificationRepository.FindByIdAsync(query.NotificationId);
    }
}
