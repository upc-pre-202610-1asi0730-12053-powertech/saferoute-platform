namespace Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

public record PhoneNumber
{
    public const int MaxLength = 20;

    public PhoneNumber() : this(string.Empty, false)
    {
    }

    public PhoneNumber(string value) : this(value, true)
    {
    }

    public PhoneNumber(string value, bool validate)
    {
        if (validate && (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength))
            throw new ArgumentException("Invalid phone number.", nameof(value));
        Value = value.Trim();
    }

    public string Value { get; init; } = string.Empty;

    public bool IsValid() => !string.IsNullOrWhiteSpace(Value) && Value.Length <= MaxLength;

    public override string ToString() => Value;
}
