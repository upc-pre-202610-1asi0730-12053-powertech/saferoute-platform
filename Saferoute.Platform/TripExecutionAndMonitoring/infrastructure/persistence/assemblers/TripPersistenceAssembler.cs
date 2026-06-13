using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;

namespace Saferoute.Platform.TripExecutionAndMonitoring.infrastructure.persistence.assemblers;

/// <summary>
/// Static assembler that maps between the <see cref="Trip"/> domain aggregate (with its <see cref="Attendance"/>
/// and <see cref="Incident"/> children) and its <see cref="TripPersistenceEntity"/> relational representation.
/// </summary>
public static class TripPersistenceAssembler
{
    /// <summary>
    /// Rebuilds a <see cref="Trip"/> domain aggregate from its persistence entity.
    /// </summary>
    /// <param name="entity">The trip persistence entity (may be null)</param>
    /// <returns>The corresponding domain trip, or null if entity is null</returns>
    public static Trip? ToDomainFromPersistence(TripPersistenceEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }
        
        var attendances = entity.Attendances
            .Select(attendanceEntity => new Attendance(
                attendanceEntity.Id,
                attendanceEntity.ChildId,
                attendanceEntity.BoardingState,
                attendanceEntity.BoardedAt))
            .ToList();

        var incidents = entity.Incidents
            .Select(incidentEntity => new Incident(
                incidentEntity.Id,
                incidentEntity.Description,
                incidentEntity.ReportedAt))
            .ToList();

        return new Trip(
            entity.Id,
            entity.OrganizationId,
            entity.RouteId,
            entity.DriverId,
            entity.TripState,
            entity.StartTime,
            entity.EndTime,
            attendances,
            incidents);
    }

    /// <summary>
    /// Maps a <see cref="Trip"/> domain aggregate to a new persistence entity ready to be saved.
    /// </summary>
    /// <param name="trip">The domain trip</param>
    /// <returns>The corresponding persistence entity, or null if trip is null</returns>
    public static TripPersistenceEntity? ToPersistenceFromDomain(Trip? trip)
    {
        if (trip is null)
        {
            return null;
        }

        var entity = new TripPersistenceEntity
        {
            OrganizationId = trip.OrganizationId,
            RouteId = trip.RouteId,
            DriverId = trip.DriverId,
            TripState = trip.TripState,
            StartTime = trip.StartTime,
            EndTime = trip.EndTime
        };

        if (trip.Id is not null)
        {
            entity.Id = trip.Id;
        }
        
        entity.Attendances = trip.Attendances
            .Select(attendance => new AttendancePersistenceEntity
            {
                Id = attendance.Id, // El set mapeará si no es null internamente
                Trip = entity,
                ChildId = attendance.ChildId,
                BoardingState = attendance.BoardingState,
                BoardedAt = attendance.BoardedAt
            })
            .ToList();

        entity.Incidents = trip.Incidents
            .Select(incident => new IncidentPersistenceEntity
            {
                Id = incident.Id,
                Trip = entity,
                Description = incident.Description,
                ReportedAt = incident.ReportedAt
            })
            .ToList();

        return entity;
    }
}