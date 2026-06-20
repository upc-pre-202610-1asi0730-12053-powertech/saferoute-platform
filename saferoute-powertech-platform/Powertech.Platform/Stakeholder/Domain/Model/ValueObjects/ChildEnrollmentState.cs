namespace Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

public record ChildEnrollmentState
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";

    public ChildEnrollmentState() : this(Active, false)
    {
    }

    public ChildEnrollmentState(string value) : this(value, true)
    {
    }

    public ChildEnrollmentState(string value, bool validate)
    {
        Value = value.Trim().ToUpperInvariant();
        if (validate && Value is not Active and not Inactive)
            throw new ArgumentException("Invalid child enrollment state.", nameof(value));
    }

    public string Value { get; init; } = Active;

    public bool IsActive() => Value == Active;

    public bool IsInactive() => Value == Inactive;

    public override string ToString() => Value;
}
