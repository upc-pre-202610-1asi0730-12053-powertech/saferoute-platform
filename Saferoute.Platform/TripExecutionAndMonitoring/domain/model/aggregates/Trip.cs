using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;

/// <summary>
/// Trip aggregate root — the heart of the Trip bounded context.
/// <para>
/// Represents the execution and monitoring of a single run of a route by a driver. 
/// The trip is the consistency boundary for its <see cref="Attendance"/> and <see cref="Incident"/>
/// child entities and for its lifecycle (<see cref="valueobjects.TripState"/>).
/// </para>
/// </summary>
public class Trip : AbstractDomainAggregateRoot<Trip>
{
    /// <summary>
    /// Persistence identity. Assigned by the infrastructure layer; null until persisted.
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Identifier of the organization the trip belongs to.
    /// </summary>
    public OrganizationId OrganizationId { get; private set; }

    /// <summary>
    /// Identifier of the route being executed.
    /// </summary>
    public RouteId RouteId { get; private set; }

    /// <summary>
    /// Identifier of the driver operating the trip.
    /// </summary>
    public DriverId DriverId { get; private set; }

    /// <summary>
    /// Current lifecycle state of the trip.
    /// </summary>
    public TripState TripState { get; private set; }

    /// <summary>
    /// Timestamp at which the trip started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Timestamp at which the trip ended, or null while in progress.
    /// </summary>
    public DateTimeOffset? EndTime { get; private set; }

    /// <summary>
    /// Boarding records for children tracked on this trip.
    /// </summary>
    private readonly List<Attendance> _attendances = new();
    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

    /// <summary>
    /// Incidents reported during this trip.
    /// </summary>
    private readonly List<Incident> _incidents = new();
    public IReadOnlyCollection<Incident> Incidents => _incidents.AsReadOnly();

    /// <summary>
    /// Default constructor required for reconstruction from persistence.
    /// </summary>
    public Trip()
    {
    }

    /// <summary>
    /// Completes the trip.
    /// </summary>
    public void Complete()
    {
        if (TripState == TripState.Completed)
        {
            throw new InvalidOperationException("Trip is already completed");
        }
        if (HasChildrenOnBoard())
        {
            throw new InvalidOperationException("Trip cannot be completed while children are still on board");
        }

        TripState = TripState.Completed;
        EndTime = DateTimeOffset.UtcNow;
        
        RegisterDomainEvent(new TripCompletedEvent(Id!.Value, RouteId.RouteIdValue, EndTime.Value));
    }

    /// <summary>
    /// Indicates whether any tracked child is currently on board.
    /// </summary>
    public bool HasChildrenOnBoard()
    {
        return _attendances.Any(a => a.IsOnBoard());
    }

    /// <summary>
    /// Returns the trip state name, matching the frontend contract.
    /// </summary>
    public string GetStateName()
    {
        return TripState.ToString();
    }
}