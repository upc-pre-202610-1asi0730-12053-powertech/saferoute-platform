namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a Trip across bounded contexts.
/// </summary>
/// <remarks>
///     The <c>Trip</c> context owns the Trip aggregate, but other contexts (e.g. Notifications)
///     reference a trip by this identifier. It is modeled in the Shared context as a stable,
///     Guid-based identity that is exposed to the frontend as a string.
/// </remarks>
/// <param name="Identifier">The unique identifier of the trip.</param>
public record TripId(Guid Identifier)
{
    /// <summary>
    ///     Parameterless constructor required by EF Core materialization and serializers.
    ///     Generates a new identifier.
    /// </summary>
    public TripId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="TripId" />.</summary>
    public static TripId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}