using System.Text.RegularExpressions;

namespace Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

public record Email
{
    public const int MaxLength = 255;

    public Email() : this(string.Empty, false)
    {
    }

    public Email(string value) : this(value, true)
    {
    }

    public Email(string value, bool validate)
    {
        if (validate && !IsValidEmail(value))
            throw new ArgumentException("Invalid email address.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }

    public string Value { get; init; } = string.Empty;

    public bool IsValid() => IsValidEmail(Value);

    public override string ToString() => Value;

    private static bool IsValidEmail(string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= MaxLength &&
        Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}
