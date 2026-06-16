namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a Driver across bounded contexts.
/// </summary>
/// <remarks>
///     A driver is a stakeholder owned by the Stakeholder/IAM context. Both <c>Fleet</c>
///     (route assignment) and <c>Trip</c> (trip operation) reference a driver through this
///     shared identifier.
/// </remarks>
/// <param name="Identifier">The unique identifier of the driver.</param>
public record DriverId(Guid Identifier)
{
    /// <summary>
    ///     Parameterless constructor required by EF Core materialization and serializers.
    ///     Generates a new identifier.
    /// </summary>
    public DriverId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="DriverId" />.</summary>
    public static DriverId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}