using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Fleet.Domain.Model.Entities;

/// <summary>
///     Entity representing a stop (waypoint) within a route's stop sequence.
/// </summary>
/// <remarks>
///     <c>Stop</c> is a child entity of the <see cref="Aggregates.Route" /> aggregate. Stops are
///     created and reordered exclusively through the aggregate root to keep the sequence consistent.
/// </remarks>
public class Stop
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Stop()
    {
        Id = new StopId();
        Name = string.Empty;
        Coordinates = new Coordinates();
        Order = new StopOrder();
    }

    /// <summary>Creates a new stop at the given position.</summary>
    /// <param name="name">The human-readable name of the stop.</param>
    /// <param name="coordinates">The geographic location of the stop.</param>
    /// <param name="order">The 1-based position of the stop in the sequence.</param>
    public Stop(string name, Coordinates coordinates, StopOrder order)
    {
        Id = StopId.New();
        Name = name;
        Coordinates = coordinates;
        Order = order;
    }

    internal Stop(StopId id, string name, Coordinates coordinates, StopOrder order)
        : this(name, coordinates, order)
    {
        Id = id;
    }

    /// <summary>Local identity of the stop within the aggregate.</summary>
    public StopId Id { get; private set; }

    /// <summary>The human-readable name of the stop.</summary>
    public string Name { get; private set; }

    /// <summary>The geographic location of the stop.</summary>
    public Coordinates Coordinates { get; private set; }

    /// <summary>The 1-based position of the stop in the route sequence.</summary>
    public StopOrder Order { get; private set; }

    /// <summary>Returns <c>true</c> when this is the first stop of the route.</summary>
    public bool IsFirst() => Order.IsFirst();

    /// <summary>Updates the geographic location of the stop.</summary>
    /// <param name="coordinates">The new coordinates.</param>
    public void UpdateCoordinates(Coordinates coordinates) => Coordinates = coordinates;

    /// <summary>Reassigns the stop to a new position in the sequence.</summary>
    /// <param name="order">The new 1-based position.</param>
    internal void Reorder(StopOrder order) => Order = order;
}
