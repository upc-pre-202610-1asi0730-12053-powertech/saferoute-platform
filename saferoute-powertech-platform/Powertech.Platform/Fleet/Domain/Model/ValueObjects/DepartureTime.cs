using System.Globalization;

namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the time of day at which a route departs.
/// </summary>
/// <remarks>
///     Persisted and exposed as an <c>HH:mm</c> string, matching the contract used by the frontend.
/// </remarks>
public record DepartureTime
{
    /// <summary>The time-of-day value.</summary>
    public TimeOnly Value { get; init; }

    /// <summary>Creates a departure time from a <see cref="TimeOnly" />.</summary>
    /// <param name="value">The time of day.</param>
    public DepartureTime(TimeOnly value)
    {
        Value = value;
    }

    /// <summary>Creates a departure time from an <c>HH:mm</c> (or parseable time) string.</summary>
    /// <param name="value">The time string, e.g. <c>07:30</c>.</param>
    /// <exception cref="ArgumentException">Thrown when the value cannot be parsed as a time of day.</exception>
    public DepartureTime(string value)
    {
        if (!TimeOnly.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            && !TimeOnly.TryParse(value, CultureInfo.InvariantCulture, out parsed))
            throw new ArgumentException($"'{value}' is not a valid departure time (expected HH:mm).", nameof(value));
        Value = parsed;
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to midnight.</summary>
    public DepartureTime() : this(new TimeOnly(0, 0))
    {
    }

    /// <summary>Returns <c>true</c> when the departure time is valid (always true once constructed).</summary>
    public bool IsValid() => true;

    /// <inheritdoc />
    public override string ToString() => Value.ToString("HH:mm", CultureInfo.InvariantCulture);
}
