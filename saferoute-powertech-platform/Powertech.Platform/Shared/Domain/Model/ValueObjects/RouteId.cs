namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a Route across bounded contexts.
/// </summary>
/// <remarks>
///     The <c>Fleet</c> context owns the Route aggregate. The <c>Trip</c> context references a
///     route through this identifier when a trip is executed over a defined route. Keeping it in
///     Shared avoids a direct dependency from Trip on the Fleet domain model.
/// </remarks>
/// <param name="Identifier">The unique identifier of the route.</param>
public record RouteId(Guid Identifier)
{
    /// <summary>
    ///     Parameterless constructor required by EF Core materialization and serializers.
    ///     Generates a new identifier.
    /// </summary>
    public RouteId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="RouteId" />.</summary>
    public static RouteId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}
