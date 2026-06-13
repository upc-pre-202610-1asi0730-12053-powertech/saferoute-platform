using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

namespace Saferoute.Platform.TripExecutionAndMonitoring.infrastructure.persistence.entities;

/// <summary>
/// Data persistence entity for trips (table <c>trips</c>).
/// <para>
/// Holds the relational mapping for the Trip aggregate. Identifier value objects are mapped
/// via converters, the lifecycle state is stored as a string enum, and the attendance and
/// incident children are owned via cascading one-to-many associations.
/// </para>
/// </summary>
public class TripPersistenceEntity : AuditableAbstractPersistenceEntity
{
    // Id heredado de AuditableAbstractPersistenceEntity (se asume tipo long?)

    public OrganizationId OrganizationId { get; set; } = null!;

    public RouteId RouteId { get; set; } = null!;

    public DriverId DriverId { get; set; } = null!;

    public TripState TripState { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    // En EF Core las relaciones One-to-Many usan ICollection para colecciones navegables
    public virtual ICollection<AttendancePersistenceEntity> Attendances { get; set; } = new List<AttendancePersistenceEntity>();

    public virtual ICollection<IncidentPersistenceEntity> Incidents { get; set; } = new List<IncidentPersistenceEntity>();
}