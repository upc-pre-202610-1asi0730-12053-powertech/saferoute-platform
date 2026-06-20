using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Queries;

namespace Powertech.Platform.Notifications.Application.QueryServices;

public interface INotificationQueryService
{
    Task<Notification?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> Handle(GetNotificationsByParentIdQuery query, CancellationToken cancellationToken = default);
}
