using Safer_Route_Platform.Fleet.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;

namespace Safer_Route_Platform.Fleet.Domain.Model.Entities;

/// <summary>
///     Entity representing the driver-and-children assignment of a route.
/// </summary>
/// <remarks>
///     <para>
///         <c>Assignment</c> is a child entity of the <see cref="Aggregates.Route" /> aggregate. It
///         binds the operating driver and the set of children assigned to the route.
///     </para>
///     <para>
///         The assigned children are stored internally as raw <see cref="System.Guid" /> values
///         (<see cref="ChildIds" />) so they map cleanly to a single persisted column, while the
///         domain API works in terms of the <see cref="ChildId" /> value object through
///         <see cref="Children" />.
///     </para>
/// </remarks>
public class Assignment
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Assignment()
    {
        Id = new AssignmentId();
        DriverId = new DriverId();
        ChildIds = [];
    }

    /// <summary>Creates a new assignment for the given driver.</summary>
    /// <param name="driverId">The driver operating the route.</param>
    public Assignment(DriverId driverId)
    {
        Id = AssignmentId.New();
        DriverId = driverId;
        ChildIds = [];
    }

    /// <summary>Local identity of the assignment.</summary>
    public AssignmentId Id { get; private set; }

    /// <summary>The driver assigned to the route.</summary>
    public DriverId DriverId { get; private set; }

    /// <summary>
    ///     The raw identifiers of the children assigned to the route. Persisted column; not part of
    ///     the public domain API (use <see cref="Children" />).
    /// </summary>
    public List<Guid> ChildIds { get; private set; }

    /// <summary>The children assigned to the route, as value objects (read-only view).</summary>
    public IReadOnlyCollection<ChildId> Children =>
        ChildIds.Select(id => new ChildId(id)).ToList();

    /// <summary>Reassigns the driver of the route.</summary>
    /// <param name="driverId">The new driver.</param>
    public void AssignDriver(DriverId driverId) => DriverId = driverId;

    /// <summary>Assigns a child to the route. Idempotent: assigning the same child twice is a no-op.</summary>
    /// <param name="childId">The child to assign.</param>
    public void AssignChild(ChildId childId)
    {
        if (!ChildIds.Contains(childId.Identifier))
            ChildIds.Add(childId.Identifier);
    }

    /// <summary>Removes a child from the route.</summary>
    /// <param name="childId">The child to remove.</param>
    public void RemoveChild(ChildId childId) => ChildIds.Remove(childId.Identifier);

    /// <summary>Returns the driver assigned to the route.</summary>
    public DriverId GetDriverId() => DriverId;

    /// <summary>Returns the number of children assigned to the route.</summary>
    public int GetChildCount() => ChildIds.Count;
}
