namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>Value object that identifies an <c>Assignment</c> entity inside the Route aggregate.</summary>
/// <param name="Identifier">The unique identifier of the assignment.</param>
public record AssignmentId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public AssignmentId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="AssignmentId" />.</summary>
    public static AssignmentId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}