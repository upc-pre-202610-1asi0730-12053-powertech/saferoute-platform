namespace Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

public record LicenseNumber
{
    public const int MaxLength = 50;

    public LicenseNumber() : this(string.Empty, false)
    {
    }

    public LicenseNumber(string value) : this(value, true)
    {
    }

    public LicenseNumber(string value, bool validate)
    {
        if (validate && (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength))
            throw new ArgumentException("Invalid license number.", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }

    public string Value { get; init; } = string.Empty;

    public bool IsValid() => !string.IsNullOrWhiteSpace(Value) && Value.Length <= MaxLength;

    public override string ToString() => Value;
}
