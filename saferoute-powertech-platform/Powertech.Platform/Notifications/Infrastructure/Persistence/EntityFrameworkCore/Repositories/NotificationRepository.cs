using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.ValueObjects;
using Powertech.Platform.Notifications.Domain.Repositories;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class NotificationRepository(AppDbContext context)
    : BaseRepository<Notification>(context), INotificationRepository
{
    public override async Task<IEnumerable<Notification>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Notification>()
            .Include(n => n.Alerts)
            .Include(n => n.Announcements)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> FindByIdAsync(Guid id)
    {
        var notificationId = new NotificationId(id);
        return await Context.Set<Notification>()
            .Include(n => n.Alerts)
            .Include(n => n.Announcements)
            .AsSplitQuery()
            .FirstOrDefaultAsync(n => n.Id == notificationId);
    }

    public async Task<IEnumerable<Notification>> FindByParentIdAsync(Guid parentId)
    {
        var parentNotificationId = new NotificationId(parentId);
        return await Context.Set<Notification>()
            .Include(n => n.Alerts)
            .Include(n => n.Announcements)
            .AsSplitQuery()
            .Where(n => n.ParentId == parentNotificationId)
            .ToListAsync();
    }
}
