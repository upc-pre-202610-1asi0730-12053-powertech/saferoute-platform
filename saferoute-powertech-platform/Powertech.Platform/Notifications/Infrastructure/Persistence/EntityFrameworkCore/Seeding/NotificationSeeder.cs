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
///     Seeds the Notifications bounded context with a couple of delivered demo notifications
///     addressed to the seeded parent, referencing the seeded trip.
/// </summary>
public static class NotificationSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Set<Notification>().AnyAsync(cancellationToken)) return;

        // The parent linked to the seeded parent@saferoute.pe IAM account.
        var seededParentEmail = new Email("parent@saferoute.pe");
        var parent = await context.Set<Parent>().Where(p => p.Email == seededParentEmail)
            .FirstOrDefaultAsync(cancellationToken);
        var trip = await context.Set<TripAggregate>().FirstOrDefaultAsync(cancellationToken);
        if (parent is null || trip is null) return;

        var organizationId = IamSeeder.SeedOrganizationId;

        var tripStarted = new Notification(new CreateNotificationCommand(organizationId,
            parent.Id.Identifier, trip.Id.Identifier, "TRIP_STARTED",
            "El bus inició la Ruta San Martín — Turno Mañana."));
        tripStarted.Dispatch();
        tripStarted.MarkDelivered();
        context.Add(tripStarted);

        var announcement = new Notification(new CreateNotificationCommand(organizationId,
            parent.Id.Identifier, trip.Id.Identifier, "ANNOUNCEMENT",
            "Bienvenido a SafeRoute: recuerde mantener actualizados los datos de sus hijos."));
        announcement.Dispatch();
        context.Add(announcement);

        await context.SaveChangesAsync(cancellationToken);
    }
}
