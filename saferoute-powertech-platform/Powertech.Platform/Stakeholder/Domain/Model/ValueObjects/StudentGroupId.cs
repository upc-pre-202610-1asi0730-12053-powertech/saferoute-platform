namespace Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

public record StudentGroupId(Guid Identifier)
{
    public StudentGroupId() : this(Guid.NewGuid())
    {
    }

    public static StudentGroupId New() => new(Guid.NewGuid());

    public override string ToString() => Identifier.ToString();
}
