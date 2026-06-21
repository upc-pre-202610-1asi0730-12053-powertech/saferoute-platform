namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a Child (student) across bounded contexts.
/// </summary>
/// <remarks>
///     A child is a stakeholder owned by the Stakeholder context. <c>Fleet</c> assigns children
///     to a route's assignment, and <c>Trip</c> tracks their boarding attendance. Both contexts
///     reference the child through this shared identifier.
/// </remarks>
/// <param name="Identifier">The unique identifier of the child.</param>
public record ChildId(Guid Identifier)
{
    /// <summary>
    ///     Parameterless constructor required by EF Core materialization and serializers.
    ///     Generates a new identifier.
    /// </summary>
    public ChildId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="ChildId" />.</summary>
    public static ChildId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}
