namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the set of weekdays on which a route operates.
/// </summary>
/// <remarks>
///     Internally stored as a canonical, comma-separated, upper-cased and de-duplicated string so it
///     maps to a single database column and benefits from value-based equality.
/// </remarks>
public record ServiceDays
{
    private static readonly string[] AllowedDays =
        ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY"];

    /// <summary>The canonical comma-separated representation of the service days.</summary>
    public string Value { get; init; }

    /// <summary>Creates a <see cref="ServiceDays" /> from a collection of weekday names.</summary>
    /// <param name="days">The weekday names (case-insensitive).</param>
    /// <exception cref="ArgumentException">Thrown when a day is not a valid weekday name.</exception>
    public ServiceDays(IEnumerable<string> days)
    {
        var normalized = days
            .Select(d => d?.Trim().ToUpperInvariant() ?? string.Empty)
            .Where(d => d.Length > 0)
            .Distinct()
            .ToList();

        var invalid = normalized.FirstOrDefault(d => !AllowedDays.Contains(d));
        if (invalid is not null)
            throw new ArgumentException($"'{invalid}' is not a valid weekday name.", nameof(days));

        // Keep canonical weekday order for stable equality and presentation.
        Value = string.Join(',', AllowedDays.Where(normalized.Contains));
    }

    /// <summary>Creates a <see cref="ServiceDays" /> from its canonical comma-separated string.</summary>
    /// <param name="value">The comma-separated weekday names.</param>
    public ServiceDays(string value)
        : this(value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to an empty set.</summary>
    public ServiceDays() : this(Array.Empty<string>())
    {
    }

    /// <summary>Returns the configured weekday names.</summary>
    public IReadOnlyList<string> GetDays() =>
        Value.Length == 0 ? [] : Value.Split(',');

    /// <summary>Returns <c>true</c> when the given day is part of the service days.</summary>
    /// <param name="day">The weekday name (case-insensitive).</param>
    public bool IncludesDay(string day) =>
        GetDays().Contains(day?.Trim().ToUpperInvariant());

    /// <summary>Returns <c>true</c> when at least one service day is configured.</summary>
    public bool IsValid() => GetDays().Count > 0;

    /// <inheritdoc />
    public override string ToString() => Value;
}
