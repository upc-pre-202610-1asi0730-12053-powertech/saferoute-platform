using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Seeds the Notifications bounded context with demo notifications and one route announcement.
/// </summary>
public static class NotificationSeeder
{
    private const string WelcomeAnnouncement =
        "Bienvenido a SafeRoute: recuerde mantener actualizados los datos de sus hijos.";

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var seededParentEmail = new Email("parent@saferoute.pe");
        var parent = await context.Set<Parent>().Where(p => p.Email == seededParentEmail)
            .FirstOrDefaultAsync(cancellationToken);
        var trip = await context.Set<TripAggregate>().FirstOrDefaultAsync(cancellationToken);
        if (parent is null || trip is null) return;

        var organizationId = IamSeeder.SeedOrganizationId;
        var notifications = await context.Set<Notification>()
            .Include(notification => notification.Announcements)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            var tripStarted = new Notification(new CreateNotificationCommand(organizationId,
                parent.Id.Identifier, trip.Id.Identifier, "TRIP_STARTED",
                "El bus inicio la Ruta San Martin - Turno Manana."));
            tripStarted.Dispatch();
            tripStarted.MarkDelivered();
            context.Add(tripStarted);
        }

        var announcement = notifications.FirstOrDefault(notification =>
            notification.Category.Value == "ANNOUNCEMENT" &&
            notification.TripId.Identifier == trip.Id.Identifier);

        if (announcement is null)
        {
            announcement = new Notification(new CreateNotificationCommand(organizationId,
                parent.Id.Identifier, trip.Id.Identifier, "ANNOUNCEMENT", WelcomeAnnouncement));
            announcement.Dispatch();
            context.Add(announcement);
        }

        if (announcement.Announcements.All(current => current.RouteId.Identifier != trip.RouteId.Identifier))
            announcement.AddAnnouncement(trip.RouteId.Identifier, WelcomeAnnouncement);

        await context.SaveChangesAsync(cancellationToken);
    }
}
