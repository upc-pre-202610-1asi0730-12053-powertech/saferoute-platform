using Safer_Route_Platform.Fleet.Domain.Model.Commands;
using Safer_Route_Platform.Fleet.Domain.Model.Entities;
using Safer_Route_Platform.Fleet.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;

namespace Safer_Route_Platform.Fleet.Domain.Model.Aggregates;

/// <summary>
///     Route Aggregate Root.
/// </summary>
/// <remarks>
///     <para>
///         A <c>Route</c> models a school transport route belonging to an organization. It is the
///         consistency boundary for its ordered <see cref="Stop" /> sequence, its assigned
///         <see cref="Vehicle" /> and its driver/children <see cref="Assignment" />.
///     </para>
///     <para>
///         A route is created in the <c>DRAFT</c> state, configured step by step (stops, vehicle,
///         driver, children, service days and departure time) and finally activated once all the
///         activation invariants are satisfied.
///     </para>
/// </remarks>
public class Route
{
    private readonly List<Stop> _stops = [];

    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Route()
    {
        Id = new RouteId();
        OrganizationId = new OrganizationId();
        Name = string.Empty;
        State = RouteState.CreateDraft();
    }

    /// <summary>Creates a new route in the <c>DRAFT</c> state.</summary>
    /// <param name="organizationId">The owning organization.</param>
    /// <param name="name">The route name.</param>
    public Route(OrganizationId organizationId, string name)
    {
        Id = RouteId.New();
        OrganizationId = organizationId;
        Name = name;
        State = RouteState.CreateDraft();
    }

    /// <summary>Creates a new route from a <see cref="CreateRouteCommand" />.</summary>
    /// <param name="command">The command carrying the route data.</param>
    public Route(CreateRouteCommand command)
        : this(new OrganizationId(command.OrganizationId), command.Name)
    {
    }

    /// <summary>The unique identity of the route.</summary>
    public RouteId Id { get; private set; }

    /// <summary>The organization that owns the route.</summary>
    public OrganizationId OrganizationId { get; private set; }

    /// <summary>The route name.</summary>
    public string Name { get; private set; }

    /// <summary>The current lifecycle state of the route.</summary>
    public RouteState State { get; private set; }

    /// <summary>The configured departure time; <c>null</c> until set.</summary>
    public DepartureTime? DepartureTime { get; private set; }

    /// <summary>The configured service days; <c>null</c> until set.</summary>
    public ServiceDays? ServiceDays { get; private set; }

    /// <summary>The vehicle assigned to the route; <c>null</c> until selected.</summary>
    public Vehicle? Vehicle { get; private set; }

    /// <summary>The driver/children assignment of the route; <c>null</c> until a driver is assigned.</summary>
    public Assignment? Assignment { get; private set; }

    /// <summary>The ordered sequence of stops (read-only view; mutate through aggregate behaviors).</summary>
    public IReadOnlyCollection<Stop> Stops => _stops;

    /// <summary>Renames the route. Only allowed while the route is a draft.</summary>
    /// <param name="name">The new route name.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void DefineRoute(string name)
    {
        EnsureDraft();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Route name cannot be empty.", nameof(name));
        Name = name;
    }

    /// <summary>Appends a stop to the end of the route sequence.</summary>
    /// <param name="name">The stop name.</param>
    /// <param name="coordinates">The stop location.</param>
    /// <returns>The created stop.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public Stop AddStop(string name, Coordinates coordinates)
    {
        EnsureDraft();
        var stop = new Stop(name, coordinates, new StopOrder(_stops.Count + 1));
        _stops.Add(stop);
        return stop;
    }

    /// <summary>Removes a stop and re-numbers the remaining stops to keep a contiguous sequence.</summary>
    /// <param name="stopId">The identity of the stop to remove.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void RemoveStop(StopId stopId)
    {
        EnsureDraft();
        var stop = _stops.FirstOrDefault(s => s.Id == stopId);
        if (stop is null) return;

        _stops.Remove(stop);
        ResequenceStops();
    }

    /// <summary>Selects the vehicle that will operate the route.</summary>
    /// <param name="vehicle">The vehicle to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void AssignVehicle(Vehicle vehicle)
    {
        EnsureDraft();
        Vehicle = vehicle;
    }

    /// <summary>Assigns the operating driver, creating the assignment when needed.</summary>
    /// <param name="driverId">The driver to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void AssignDriver(DriverId driverId)
    {
        EnsureDraft();
        if (Assignment is null)
            Assignment = new Assignment(driverId);
        else
            Assignment.AssignDriver(driverId);
    }

    /// <summary>Assigns a child to the route's assignment.</summary>
    /// <param name="childId">The child to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown when no driver has been assigned yet.</exception>
    public void AssignChild(ChildId childId)
    {
        EnsureDraft();
        if (Assignment is null)
            throw new InvalidOperationException("A driver must be assigned before assigning children.");
        Assignment.AssignChild(childId);
    }

    /// <summary>Removes a child from the route's assignment.</summary>
    /// <param name="childId">The child to remove.</param>
    public void RemoveChild(ChildId childId)
    {
        EnsureDraft();
        Assignment?.RemoveChild(childId);
    }

    /// <summary>Defines the weekdays on which the route operates.</summary>
    /// <param name="serviceDays">The service days.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void DefineServiceDays(ServiceDays serviceDays)
    {
        EnsureDraft();
        ServiceDays = serviceDays;
    }

    /// <summary>Sets the route departure time.</summary>
    /// <param name="departureTime">The departure time.</param>
    /// <exception cref="InvalidOperationException">Thrown when the route is not a draft.</exception>
    public void SetDepartureTime(DepartureTime departureTime)
    {
        EnsureDraft();
        DepartureTime = departureTime;
    }

    /// <summary>
    ///     Finalizes the route setup and activates it. Requires the full configuration to be present.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the route is not a draft or its setup is incomplete.
    /// </exception>
    public void Activate()
    {
        if (!State.IsDraft())
            throw new InvalidOperationException("Only a draft route can be activated.");
        if (_stops.Count == 0)
            throw new InvalidOperationException("A route must have at least one stop to be activated.");
        if (Vehicle is null)
            throw new InvalidOperationException("A route must have a vehicle assigned to be activated.");
        if (Assignment is null)
            throw new InvalidOperationException("A route must have a driver assigned to be activated.");
        if (ServiceDays is null || !ServiceDays.IsValid())
            throw new InvalidOperationException("A route must have at least one service day to be activated.");
        if (DepartureTime is null)
            throw new InvalidOperationException("A route must have a departure time to be activated.");

        State = new RouteState(RouteState.Active);
    }

    /// <summary>Deactivates an active route.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the route is not active.</exception>
    public void Deactivate()
    {
        if (!State.IsActive())
            throw new InvalidOperationException("Only an active route can be deactivated.");
        State = new RouteState(RouteState.Inactive);
    }

    /// <summary>Returns the ordered stop sequence.</summary>
    public IReadOnlyList<Stop> GetStopSequence() =>
        _stops.OrderBy(s => s.Order.Position).ToList();

    /// <summary>Returns the total number of stops.</summary>
    public int GetTotalStops() => _stops.Count;

    /// <summary>Guards that the route is still a draft before a configuration change.</summary>
    private void EnsureDraft()
    {
        if (!State.IsDraft())
            throw new InvalidOperationException("The route can only be modified while it is a draft.");
    }

    /// <summary>Re-numbers stops to a contiguous 1-based sequence after a removal.</summary>
    private void ResequenceStops()
    {
        var ordered = _stops.OrderBy(s => s.Order.Position).ToList();
        for (var i = 0; i < ordered.Count; i++)
            ordered[i].Reorder(new StopOrder(i + 1));
    }
}
