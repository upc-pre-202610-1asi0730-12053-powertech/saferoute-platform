using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Repositories;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class NotificationRepository(AppDbContext context)
    : BaseRepository<Notification>(context), INotificationRepository
{
    public async Task<Notification?> FindByIdAsync(Guid id)
    {
        return await Context.Set<Notification>()
            .Include(n => n.Alerts)
            .Include(n => n.Announcements)
            .FirstOrDefaultAsync(n => n.Id.Identifier == id);
    }

    public async Task<IEnumerable<Notification>> FindByParentIdAsync(Guid parentId)
    {
        return await Context.Set<Notification>()
            .Include(n => n.Alerts)
            .Include(n => n.Announcements)
            .Where(n => n.ParentId.Identifier == parentId)
            .ToListAsync();
    }
}
