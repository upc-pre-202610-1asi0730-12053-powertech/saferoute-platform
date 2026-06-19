using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Domain.Model.Entities;
using Powertech.Platform.Trip.Domain.Model.ValueObjects;

namespace Powertech.Platform.Trip.Domain.Model.Aggregates;

/// <summary>
///     Trip Aggregate Root.
/// </summary>
/// <remarks>
///     <para>
///         A <c>Trip</c> represents the execution of a route by a driver on behalf of an
///         organization. It is the consistency boundary for boarding <see cref="Attendance" />
///         records and reported <see cref="Incident" />s.
///     </para>
///     <para>
///         The aggregate enforces its lifecycle invariants (PENDING → IN_PROGRESS → COMPLETED) and
///         is the only entry point through which its child entities may be created or mutated. The
///         aggregate receives commands but does not dispatch them — that is the responsibility of
///         the application-layer command service.
///     </para>
/// </remarks>
public class Trip
{
    private readonly List<Attendance> _attendances = [];
    private readonly List<Incident> _incidents = [];
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Trip()
    {
        Id = new TripId();
        OrganizationId = new OrganizationId();
        RouteId = new RouteId();
        DriverId = new DriverId();
        State = TripState.CreatePending();
    }

    /// <summary>
    ///     Creates a new trip in the <c>PENDING</c> state from the provided references.
    /// </summary>
    /// <param name="organizationId">The owning organization.</param>
    /// <param name="routeId">The route to be executed.</param>
    /// <param name="driverId">The operating driver.</param>
    public Trip(OrganizationId organizationId, RouteId routeId, DriverId driverId)
    {
        Id = TripId.New();
        OrganizationId = organizationId;
        RouteId = routeId;
        DriverId = driverId;
        State = TripState.CreatePending();
    }
    
    /// <summary>
    ///     Creates a new trip from a <see cref="CreateTripCommand" />.
    /// </summary>
    /// <param name="command">The command carrying the primitive identifiers.</param>
    public Trip(CreateTripCommand command)
        : this(new OrganizationId(command.OrganizationId), new RouteId(command.RouteId), new DriverId(command.DriverId))
    {
    }

    /// <summary>The unique identity of the trip.</summary>
    public TripId Id { get; private set; }

    /// <summary>The organization that owns the trip.</summary>
    public OrganizationId OrganizationId { get; private set; }

    /// <summary>The route the trip is executed over.</summary>
    public RouteId RouteId { get; private set; }

    /// <summary>The driver operating the trip.</summary>
    public DriverId DriverId { get; private set; }

    /// <summary>The current lifecycle state of the trip.</summary>
    public TripState State { get; private set; }

    /// <summary>The moment the trip started; <c>null</c> while pending.</summary>
    public DateTimeOffset? StartTime { get; private set; }

    /// <summary>The moment the trip completed; <c>null</c> until completion.</summary>
    public DateTimeOffset? EndTime { get; private set; }
    
    /// <summary>
    ///     The boarding attendance records collected during the trip. Exposed as a read-only view;
    ///     mutation happens only through <see cref="SetBoardingStatus" />. The backing field is the
    ///     navigation EF Core maps as an owned collection.
    /// </summary>
    public IReadOnlyCollection<Attendance> Attendances => _attendances;
    
    /// <summary>
    ///     The incidents reported during the trip. Exposed as a read-only view; mutation happens
    ///     only through <see cref="ReportIncident" />.
    /// </summary>
    public IReadOnlyCollection<Incident> Incidents => _incidents;

    /// <summary>
    ///     Starts the trip, transitioning it to <c>IN_PROGRESS</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the trip is not in the pending state.</exception>
    public void Start()
    {
        if (!State.IsPending())
            throw new InvalidOperationException("Only a pending trip can be started.");

        State = new TripState(TripState.InProgress);
        StartTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Completes the trip, transitioning it to <c>COMPLETED</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the trip is not in progress.</exception>
    public void Complete()
    {
        if (!State.IsInProgress())
            throw new InvalidOperationException("Only an in-progress trip can be completed.");

        State = new TripState(TripState.Completed);
        EndTime = DateTimeOffset.UtcNow;
    }
    
    public void SetBoardingStatus(ChildId childId, BoardingState state)
    {
        if (!State.IsInProgress())
            throw new InvalidOperationException("Boarding can only be recorded while the trip is in progress.");

        var existing = _attendances.FirstOrDefault(a => a.ChildId == childId);
        if (existing is not null)
            existing.UpdateBoardingState(state);
        else
            _attendances.Add(new Attendance(childId, state));
    }
    
    /// <summary>
    ///     Reports an incident during the trip.
    /// </summary>
    /// <param name="description">The validated incident description.</param>
    /// <exception cref="InvalidOperationException">Thrown when the trip is not in progress.</exception>
    public void ReportIncident(IncidentDescription description)
    {
        if (!State.IsInProgress())
            throw new InvalidOperationException("Incidents can only be reported while the trip is in progress.");

        _incidents.Add(new Incident(description));
    }

    /// <summary>Returns the boarding attendance summary for the trip.</summary>
    public IReadOnlyCollection<Attendance> GetAttendanceSummary() => Attendances;
    
    /// <summary>Returns the incident log for the trip.</summary>
    public IReadOnlyCollection<Incident> GetIncidentLog() => Incidents;
    
    /// <summary>Returns <c>true</c> when the trip is currently in progress.</summary>
    public bool IsInProgress() => State.IsInProgress();
}
