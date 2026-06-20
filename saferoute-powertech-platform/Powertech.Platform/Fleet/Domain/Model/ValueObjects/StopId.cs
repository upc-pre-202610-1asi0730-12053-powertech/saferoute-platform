namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>Value object that identifies a <c>Stop</c> entity inside the Route aggregate.</summary>
/// <param name="Identifier">The unique identifier of the stop.</param>
public record StopId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public StopId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="StopId" />.</summary>
    public static StopId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}
