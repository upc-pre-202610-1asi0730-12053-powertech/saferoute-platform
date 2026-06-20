namespace Powertech.Platform.Subscription.Domain.Model.ValueObjects;

public record PlanTier
{
    public const string Basic = "BASIC";
    public const string Intermediate = "INTERMEDIATE";
    public const string Complete = "COMPLETE";

    public PlanTier() : this(Basic, false)
    {
    }

    public PlanTier(string value) : this(value, true)
    {
    }

    public PlanTier(string value, bool validate)
    {
        Value = value.Trim().ToUpperInvariant();
        if (validate && Value is not Basic and not Intermediate and not Complete)
            throw new ArgumentException("Invalid plan tier.", nameof(value));
    }

    public string Value { get; init; } = Basic;

    public bool IsBasic() => Value == Basic;

    public bool IsIntermediate() => Value == Intermediate;

    public bool IsComplete() => Value == Complete;

    public override string ToString() => Value;
}
