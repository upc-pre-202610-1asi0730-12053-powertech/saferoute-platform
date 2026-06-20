namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>Value object that identifies a <c>Vehicle</c> entity inside the Route aggregate.</summary>
/// <param name="Identifier">The unique identifier of the vehicle.</param>
public record VehicleId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public VehicleId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="VehicleId" />.</summary>
    public static VehicleId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}
