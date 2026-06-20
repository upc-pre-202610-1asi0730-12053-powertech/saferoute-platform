namespace Powertech.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the lifecycle state of a <c>Route</c>.
/// </summary>
/// <remarks>
///     A route is created as a <c>DRAFT</c>, becomes <c>ACTIVE</c> once its setup is finalized and
///     can later be set <c>INACTIVE</c>.
/// </remarks>
public record RouteState
{
    /// <summary>The route is being configured and is not yet operational.</summary>
    public const string Draft = "DRAFT";

    /// <summary>The route is active and can be operated.</summary>
    public const string Active = "ACTIVE";

    /// <summary>The route is no longer operated.</summary>
    public const string Inactive = "INACTIVE";

    private static readonly string[] AllowedValues = [Draft, Active, Inactive];

    /// <summary>The raw state value.</summary>
    public string Value { get; init; }

    /// <summary>Creates a new <see cref="RouteState" />, validating it against the allowed values.</summary>
    /// <param name="value">One of <see cref="Draft" />, <see cref="Active" />, <see cref="Inactive" />.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not a known route state.</exception>
    public RouteState(string value)
    {
        if (!AllowedValues.Contains(value))
            throw new ArgumentException($"'{value}' is not a valid route state.", nameof(value));
        Value = value;
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to <see cref="Draft" />.</summary>
    public RouteState() : this(Draft)
    {
    }

    /// <summary>Factory for the initial state of a newly defined route.</summary>
    public static RouteState CreateDraft() => new(Draft);

    /// <summary>Returns <c>true</c> when the route is still a draft.</summary>
    public bool IsDraft() => Value == Draft;

    /// <summary>Returns <c>true</c> when the route is active.</summary>
    public bool IsActive() => Value == Active;

    /// <summary>Returns <c>true</c> when the route is inactive.</summary>
    public bool IsInactive() => Value == Inactive;

    /// <inheritdoc />
    public override string ToString() => Value;
}
