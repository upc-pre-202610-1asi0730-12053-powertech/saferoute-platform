using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Trip.Domain.Model.ValueObjects;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     EF Core model configuration for the Trip bounded context.
/// </summary>
/// <remarks>
///     <para>
///         Maps the Guid-based identity and reference value objects through value converters and
///         configures the aggregate's child entities (<c>Attendance</c>, <c>Incident</c>) as owned
///         collections, so they share the aggregate's lifecycle and live inside its boundary.
///     </para>
///     <para>
///         Column, table and key names are normalized to snake_case by the global naming convention
///         applied in <c>AppDbContext.OnModelCreating</c>, so only structural mapping is declared here.
///     </para>
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the Trip context configuration to the EF Core model.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyTripConfiguration(this ModelBuilder builder)
    {
        builder.Entity<TripAggregate>(trip =>
        {
            // Identity (Guid value object, application-assigned).
            trip.HasKey(t => t.Id);
            trip.Property(t => t.Id)
                .HasConversion(id => id.Identifier, value => new TripId(value))
                .ValueGeneratedNever()
                .IsRequired();

            // Cross-context references (Shared value objects).
            trip.Property(t => t.OrganizationId)
                .HasConversion(id => id.Identifier, value => new OrganizationId(value))
                .IsRequired();

            trip.Property(t => t.RouteId)
                .HasConversion(id => id.Identifier, value => new RouteId(value))
                .IsRequired();

            trip.Property(t => t.DriverId)
                .HasConversion(id => id.Identifier, value => new DriverId(value))
                .IsRequired();

            // Lifecycle state, persisted as its string value.
            trip.Property(t => t.State)
                .HasConversion(state => state.Value, value => new TripState(value))
                .IsRequired();

            trip.Property(t => t.StartTime);
            trip.Property(t => t.EndTime);
        });
    }
}