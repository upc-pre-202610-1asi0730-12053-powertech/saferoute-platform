namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the 1-based position of a stop within a route's stop sequence.
/// </summary>
/// <param name="Position">The 1-based position of the stop.</param>
public record StopOrder
{
    /// <summary>The 1-based position of the stop.</summary>
    public int Position { get; init; }

    /// <summary>Creates a new <see cref="StopOrder" />, enforcing a positive position.</summary>
    /// <param name="position">The 1-based position; must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the position is not positive.</exception>
    public StopOrder(int position)
    {
        if (position < 1)
            throw new ArgumentOutOfRangeException(nameof(position), "Stop order must be a positive 1-based position.");
        Position = position;
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to the first position.</summary>
    public StopOrder() : this(1)
    {
    }

    /// <summary>Returns <c>true</c> when this is the first stop of the route.</summary>
    public bool IsFirst() => Position == 1;

    /// <summary>Returns the 1-based position.</summary>
    public int GetPosition() => Position;

    /// <inheritdoc />
    public override string ToString() => Position.ToString();
}