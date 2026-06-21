namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a Profile (a driver or parent stakeholder profile).
/// </summary>
/// <param name="Identifier">The unique identifier of the profile.</param>
public record ProfileId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization. Generates a new identifier.</summary>
    public ProfileId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="ProfileId" />.</summary>
    public static ProfileId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}
