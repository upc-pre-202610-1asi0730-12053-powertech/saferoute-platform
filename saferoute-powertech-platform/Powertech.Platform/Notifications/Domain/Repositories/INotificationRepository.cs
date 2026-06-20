using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Shared.Domain.Repositories;

namespace Powertech.Platform.Notifications.Domain.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<Notification?> FindByIdAsync(Guid id);
}
