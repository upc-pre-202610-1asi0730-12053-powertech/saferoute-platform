using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.ValueObjects;

namespace Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Notification>(notification =>
        {
            notification.ToTable("Notifications");
            notification.HasKey(n => n.Id);
            notification.Property(n => n.Id)
                .HasConversion(id => id.Identifier, value => new NotificationId(value))
                .ValueGeneratedNever();
            notification.Property(n => n.OrganizationId)
                .HasConversion(id => id.Identifier, value => new NotificationId(value));
            notification.Property(n => n.ParentId)
                .HasConversion(id => id.Identifier, value => new NotificationId(value));
            notification.Property(n => n.TripId)
                .HasConversion(id => id.Identifier, value => new NotificationId(value));
            notification.Property(n => n.Category)
                .HasConversion(category => category.Value, value => new NotificationCategory(value))
                .HasMaxLength(50).IsRequired();
            notification.Property(n => n.DeliveryState)
                .HasConversion(state => state.Value, value => new NotificationDeliveryState(value))
                .HasMaxLength(20).IsRequired();
            notification.Property(n => n.Message)
                .HasConversion(message => message.Content, value => new NotificationMessage(value))
                .HasMaxLength(1000).IsRequired();
            notification.Property(n => n.SentAt).IsRequired();

            // Alerts Collection
            notification.OwnsMany(n => n.Alerts, alert =>
            {
                alert.WithOwner().HasForeignKey("OwnerNotificationId");
                alert.HasKey(a => a.Id);
                alert.Property(a => a.Id)
                    .HasConversion(id => id.Identifier, value => new AlertId(value))
                    .ValueGeneratedNever();
                alert.Property(a => a.NotificationId)
                    .HasConversion(id => id.Identifier, value => new NotificationId(value));
                alert.Property(a => a.TriggeredAt).IsRequired();
                alert.Property(a => a.Panic).IsRequired();
            });

            // Announcements Collection
            notification.OwnsMany(n => n.Announcements, announcement =>
            {
                announcement.WithOwner().HasForeignKey("OwnerNotificationId");
                announcement.HasKey(a => a.Id);
                announcement.Property(a => a.Id)
                    .HasConversion(id => id.Identifier, value => new AnnouncementId(value))
                    .ValueGeneratedNever();
                announcement.Property(a => a.NotificationId)
                    .HasConversion(id => id.Identifier, value => new NotificationId(value));
                announcement.Property(a => a.RouteId)
                    .HasConversion(id => id.Identifier, value => new NotificationId(value));
                announcement.Property(a => a.Message)
                    .HasConversion(message => message.Content, value => new NotificationMessage(value))
                    .HasMaxLength(1000).IsRequired();
                announcement.Property(a => a.PublishedAt).IsRequired();
            });
        });
    }
}
