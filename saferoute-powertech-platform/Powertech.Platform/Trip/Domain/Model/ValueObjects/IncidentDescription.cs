namespace Powertech.Platform.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Value object holding the textual description of an incident reported during a trip.
/// </summary>
/// <remarks>
///     Enforces length invariants so an incident always carries a meaningful description
///     (between 10 and 500 characters).
/// </remarks>
public record IncidentDescription
{
    /// <summary>Minimum number of characters required for a valid description.</summary>
    public const int MinLength = 10;

    /// <summary>Maximum number of characters allowed for a description.</summary>
    public const int MaxLength = 500;

    /// <summary>The description text.</summary>
    public string Value { get; init; }

    /// <summary>
    ///     Creates a new <see cref="IncidentDescription" /> validating its content and length.
    /// </summary>
    /// <param name="value">The incident description text.</param>
    /// <exception cref="ArgumentException">Thrown when the description is empty or out of bounds.</exception>
    public IncidentDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("The incident description cannot be empty.", nameof(value));
        if (value.Length < MinLength)
            throw new ArgumentException(
                $"The incident description is too short (minimum {MinLength} characters).", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException(
                $"The incident description cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public IncidentDescription() : this(new string('-', MinLength))
    {
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
