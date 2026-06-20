using Powertech.Platform.Fleet.Domain.Model.Entities;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Fleet.Domain.Model.Aggregates;

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

    /// <summary>The unique identity of the route.</summary>
    public RouteId Id { get; private set; }

    /// <summary>The organization that owns the route.</summary>
    public OrganizationId OrganizationId { get; private set; }

    /// <summary>The route name.</summary>
    public string Name { get; private set; }

    /// <summary>The current lifecycle state of the route.</summary>
    public RouteState State { get; private set; }

    /// <summary>The driver/children assignment of the route; <c>null</c> until a driver is assigned.</summary>
    public Assignment? Assignment { get; private set; }

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

    /// <summary>Assigns a student to the route's assignment.</summary>
    /// <param name="childId">The child to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown when no driver has been assigned yet.</exception>
    public void AssignStudent(ChildId childId)
    {
        EnsureDraft();
        if (Assignment is null)
            throw new InvalidOperationException("A driver must be assigned before assigning children.");
        Assignment.AssignStudent(childId);
    }

    /// <summary>Removes a student from the route's assignment.</summary>
    /// <param name="childId">The child to remove.</param>
    public void RemoveStudent(ChildId childId)
    {
        EnsureDraft();
        Assignment?.RemoveStudent(childId);
    }
    
    /// <summary>Guards that the route is still a draft before a configuration change.</summary>
    private void EnsureDraft()
    {
        if (!State.IsDraft())
            throw new InvalidOperationException("The route can only be modified while it is a draft.");
    }

}