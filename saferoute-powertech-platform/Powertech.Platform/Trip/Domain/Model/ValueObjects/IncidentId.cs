namespace Powertech.Platform.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Value object that identifies an <c>Incident</c> entity inside the Trip aggregate.
/// </summary>
/// <remarks>
///     Local identity of a child entity within the Trip aggregate boundary.
/// </remarks>
/// <param name="Identifier">The unique identifier of the incident.</param>
public record IncidentId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public IncidentId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="IncidentId" />.</summary>
    public static IncidentId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}