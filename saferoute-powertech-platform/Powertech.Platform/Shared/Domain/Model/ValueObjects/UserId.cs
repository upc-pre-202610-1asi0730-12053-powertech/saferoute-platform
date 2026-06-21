namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a user across bounded contexts.
/// </summary>
/// <param name="Identifier">The unique identifier of the user.</param>
public record UserId(Guid Identifier)
{
    public UserId() : this(Guid.NewGuid())
    {
    }

    public static UserId New() => new(Guid.NewGuid());

    public override string ToString() => Identifier.ToString();
}
