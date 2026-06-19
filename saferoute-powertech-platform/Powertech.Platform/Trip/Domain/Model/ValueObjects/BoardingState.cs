namespace Powertech.Platform.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the boarding status of a child during a trip.
/// </summary>
/// <remarks>
///     Reflects the boarding outcome recorded by the driver: the child <c>BOARDED</c> the
///     vehicle, was <c>MISSING</c> at the stop, or was deliberately <c>OMITTED</c> from the trip.
/// </remarks>
public record BoardingState
{
    /// <summary>The child boarded the vehicle.</summary>
    public const string Boarded = "BOARDED";

    /// <summary>The child was not present at the stop.</summary>
    public const string Missing = "MISSING";

    /// <summary>The child was intentionally omitted from this trip.</summary>
    public const string Omitted = "OMITTED";

    private static readonly string[] AllowedValues = [Boarded, Missing, Omitted];

    /// <summary>The raw boarding state value.</summary>
    public string Value { get; init; }

    /// <summary>
    ///     Creates a new <see cref="BoardingState" /> validating it against the allowed values.
    /// </summary>
    /// <param name="value">One of <see cref="Boarded" />, <see cref="Missing" />, <see cref="Omitted" />.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not a known boarding state.</exception>
    public BoardingState(string value)
    {
        if (!AllowedValues.Contains(value))
            throw new ArgumentException($"'{value}' is not a valid boarding state.", nameof(value));
        Value = value;
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to <see cref="Missing" />.</summary>
    public BoardingState() : this(Missing)
    {
    }

    /// <summary>Returns <c>true</c> when the child boarded the vehicle.</summary>
    public bool IsBoarded() => Value == Boarded;

    /// <summary>Returns <c>true</c> when the child was missing at the stop.</summary>
    public bool IsMissing() => Value == Missing;

    /// <summary>Returns <c>true</c> when the child was omitted from the trip.</summary>
    public bool IsOmitted() => Value == Omitted;

    /// <inheritdoc />
    public override string ToString() => Value;
}
