namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a parent across bounded contexts.
/// </summary>
/// <param name="Identifier">The unique identifier of the parent.</param>
public record ParentId(Guid Identifier)
{
    public ParentId() : this(Guid.NewGuid())
    {
    }

    public static ParentId New() => new(Guid.NewGuid());

    public override string ToString() => Identifier.ToString();
}
