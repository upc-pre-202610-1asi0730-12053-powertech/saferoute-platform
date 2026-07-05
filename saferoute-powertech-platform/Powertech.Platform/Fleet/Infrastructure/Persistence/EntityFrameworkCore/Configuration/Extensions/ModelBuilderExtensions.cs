using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     EF Core model configuration for the Fleet bounded context.
/// </summary>
/// <remarks>
///     <para>
///         Maps the Route aggregate root, its Guid identity and reference value objects, and its
///         child entities (<c>Stop</c> as an owned collection; <c>Vehicle</c> and <c>Assignment</c>
///         as optional owned references that share the route table).
///     </para>
///     <para>
///         The assigned children of an assignment are stored as a single comma-separated column
///         through a value converter and comparer, avoiding an extra link table while keeping the
///         data inside the aggregate boundary. Snake_case naming is applied globally by the context.
///     </para>
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>Applies the Fleet context configuration to the EF Core model.</summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyFleetConfiguration(this ModelBuilder builder)
    {
        // Converter/comparer that persists the assigned children as a comma-separated Guid string.
        var childIdsConverter = new ValueConverter<List<Guid>, string>(
            ids => string.Join(',', ids),
            value => value.Length == 0
                ? new List<Guid>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList());

        var childIdsComparer = new ValueComparer<List<Guid>>(
            (left, right) => (left ?? new List<Guid>()).SequenceEqual(right ?? new List<Guid>()),
            ids => ids.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.GetHashCode())),
            ids => ids.ToList());

        builder.Entity<Route>(route =>
        {
            // Identity (Guid value object, application-assigned).
            route.HasKey(r => r.Id);
            route.Property(r => r.Id)
                .HasConversion(id => id.Identifier, value => new RouteId(value))
                .ValueGeneratedNever()
                .IsRequired();

            route.Property(r => r.OrganizationId)
                .HasConversion(id => id.Identifier, value => new OrganizationId(value))
                .IsRequired();

            route.Property(r => r.Name).IsRequired().HasMaxLength(120);

            route.Property(r => r.State)
                .HasConversion(state => state.Value, value => new RouteState(value))
                .IsRequired();

            // Optional schedule value objects, persisted as their canonical string forms.
            route.Property(r => r.DepartureTime)
                .HasConversion(time => time == null ? null : time.ToString(),
                    value => value == null ? null : new DepartureTime(value));

            route.Property(r => r.ServiceDays)
                .HasConversion(days => days == null ? null : days.Value,
                    value => value == null ? null : new ServiceDays(value));

            // Owned collection in its own table: the ordered stop sequence.
            route.OwnsMany(r => r.Stops, stop =>
            {
                stop.ToTable("RouteStops");
                stop.WithOwner().HasForeignKey("RouteId");

                stop.Property(s => s.Id)
                    .HasConversion(id => id.Identifier, value => new StopId(value))
                    .ValueGeneratedNever();
                stop.HasKey(s => s.Id);

                stop.Property(s => s.Name).IsRequired().HasMaxLength(120);

                stop.Property(s => s.Order)
                    .HasConversion(order => order.Position, value => new StopOrder(value))
                    .IsRequired();

                stop.OwnsOne(s => s.Coordinates, coordinates =>
                {
                    coordinates.WithOwner().HasForeignKey("Id");
                    coordinates.Property(c => c.Latitude).HasColumnName("latitude").IsRequired();
                    coordinates.Property(c => c.Longitude).HasColumnName("longitude").IsRequired();
                });
            });

            // Optional owned single in its own table: the vehicle operating the route.
            route.OwnsOne(r => r.Vehicle, vehicle =>
            {
                vehicle.ToTable("RouteVehicle");
                vehicle.WithOwner().HasForeignKey("RouteId");

                vehicle.Property(v => v.Id)
                    .HasConversion(id => id.Identifier, value => new VehicleId(value))
                    .ValueGeneratedNever();
                vehicle.HasKey(v => v.Id);

                vehicle.Property(v => v.OrganizationId)
                    .HasConversion(id => id.Identifier, value => new OrganizationId(value));

                vehicle.Property(v => v.Plate).IsRequired().HasMaxLength(20);
                vehicle.Property(v => v.Model).IsRequired().HasMaxLength(80);
                vehicle.Property(v => v.Brand).IsRequired().HasMaxLength(80);
                vehicle.Property(v => v.Capacity).IsRequired();
            });

            // Optional owned single in its own table: the driver/children assignment. Like the
            // vehicle, it owns its identity (AssignmentId) and therefore lives in its own
            // 'route_assignments' table with a foreign key back to the route.
            route.OwnsOne(r => r.Assignment, assignment =>
            {
                assignment.ToTable("RouteAssignment");
                assignment.WithOwner().HasForeignKey("RouteId");

                assignment.Property(a => a.Id)
                    .HasConversion(id => id.Identifier, value => new AssignmentId(value))
                    .ValueGeneratedNever();
                assignment.HasKey(a => a.Id);

                assignment.Property(a => a.DriverId)
                    .HasConversion(id => id.Identifier, value => new DriverId(value));

                var childIdsProperty = assignment.Property(a => a.ChildIds)
                    .HasConversion(childIdsConverter)
                    .HasColumnName("child_ids")
                    .Metadata;
                childIdsProperty.SetValueComparer(childIdsComparer);

                // Computed read-only view; not persisted.
                assignment.Ignore(a => a.Children);
            });
        });
    }
}