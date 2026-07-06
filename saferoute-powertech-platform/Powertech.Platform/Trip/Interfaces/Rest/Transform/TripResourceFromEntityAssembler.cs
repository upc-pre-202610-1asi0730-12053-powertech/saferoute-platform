using Powertech.Platform.Trip.Domain.Model.Entities;
using Powertech.Platform.Trip.Interfaces.Rest.Resources;

namespace Powertech.Platform.Trip.Interfaces.Rest.Transform;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

/// <summary>
///     Assembler that converts a <see cref="TripAggregate" /> into its <see cref="TripResource" />
///     representation, including its child attendances and incidents.
/// </summary>
public static class TripResourceFromEntityAssembler
{
    /// <summary>Converts the aggregate into the published trip resource.</summary>
    /// <param name="entity">The trip aggregate. Must not be null.</param>
    /// <returns>The resulting <see cref="TripResource" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity" /> is null.</exception>
    public static TripResource ToResourceFromEntity(TripAggregate entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new TripResource(
            entity.Id.ToString(),
            entity.OrganizationId.ToString(),
            entity.RouteId.ToString(),
            entity.DriverId.ToString(),
            entity.State.Value,
            entity.StartTime,
            entity.EndTime,
            entity.Attendances.Select(ToAttendanceResource).ToList(),
            entity.Incidents.Select(ToIncidentResource).ToList());
    }
    
    /// <summary>Converts an <see cref="Attendance" /> entity into its resource.</summary>
    private static AttendanceResource ToAttendanceResource(Attendance attendance) =>
        new(attendance.Id.ToString(),
            attendance.ChildId.ToString(),
            attendance.BoardingState.Value,
            attendance.BoardedAt);

    /// <summary>Converts an <see cref="Incident" /> entity into its resource.</summary>
    private static IncidentResource ToIncidentResource(Incident incident) =>
        new(incident.Id.ToString(),
            incident.Description.Value,
            incident.ReportedAt);
}
