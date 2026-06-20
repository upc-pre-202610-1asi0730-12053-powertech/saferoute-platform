using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;

namespace Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Notification>().HasKey(n => n.Id);
        
        builder.Entity<Notification>().OwnsOne(n => n.Id,
            ni => { ni.Property(n => n.Identifier).HasColumnName("Id").IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.OrganizationId,
            ni => { ni.Property(n => n.Identifier).HasColumnName("OrganizationId").IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.ParentId,
            ni => { ni.Property(n => n.Identifier).HasColumnName("ParentId").IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.TripId,
            ni => { ni.Property(n => n.Identifier).HasColumnName("TripId").IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.Category,
            ni => { ni.Property(n => n.Value).HasColumnName("Category").HasMaxLength(50).IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.DeliveryState,
            ni => { ni.Property(n => n.Value).HasColumnName("DeliveryState").HasMaxLength(20).IsRequired(); });
            
        builder.Entity<Notification>().OwnsOne(n => n.Message,
            ni => { ni.Property(n => n.Content).HasColumnName("Message").HasMaxLength(1000).IsRequired(); });
            
        builder.Entity<Notification>().Property(n => n.SentAt).IsRequired();

        // Alerts Collection
        builder.Entity<Notification>().OwnsMany(n => n.Alerts, a =>
        {
            a.WithOwner().HasForeignKey("NotificationId");
            a.HasKey("Id");
            a.Property(al => al.Id).ValueGeneratedOnAdd();
            a.Property(al => al.TriggeredAt).IsRequired();
            a.Property(al => al.Panic).IsRequired();
        });

        // Announcements Collection
        builder.Entity<Notification>().OwnsMany(n => n.Announcements, an =>
        {
            an.WithOwner().HasForeignKey("NotificationId");
            an.HasKey("Id");
            an.Property(ann => ann.Id).ValueGeneratedOnAdd();
            an.Property(ann => ann.PublishedAt).IsRequired();
            an.OwnsOne(ann => ann.RouteId, 
                ri => { ri.Property(r => r.Identifier).HasColumnName("RouteId").IsRequired(); });
            an.OwnsOne(ann => ann.Message, 
                mi => { mi.Property(m => m.Content).HasColumnName("Message").HasMaxLength(1000).IsRequired(); });
        });
    }
}
