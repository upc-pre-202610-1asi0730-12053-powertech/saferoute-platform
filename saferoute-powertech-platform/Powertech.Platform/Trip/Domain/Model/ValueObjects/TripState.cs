namespace Powertech.Platform.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the lifecycle state of a <c>Trip</c>.
/// </summary>
/// <remarks>
///     A trip transitions <c>PENDING → IN_PROGRESS → COMPLETED</c>. The value object guards the
///     set of valid states so the aggregate can never hold an unknown state. The state string is
///     the contract exposed to the frontend through the Trip resource.
/// </remarks>
public record TripState
{
    /// <summary>The trip session has been prepared but not started.</summary>
    public const string Pending = "PENDING";

    /// <summary>The trip is currently being executed by the driver.</summary>
    public const string InProgress = "IN_PROGRESS";

    /// <summary>The trip has finished and is archived.</summary>
    public const string Completed = "COMPLETED";

    private static readonly string[] AllowedValues = [Pending, InProgress, Completed];

    /// <summary>The raw state value.</summary>
    public string Value { get; init; }

    /// <summary>
    ///     Creates a new <see cref="TripState" /> validating it against the allowed values.
    /// </summary>
    /// <param name="value">One of <see cref="Pending" />, <see cref="InProgress" />, <see cref="Completed" />.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not a known trip state.</exception>
    public TripState(string value)
    {
        if (!AllowedValues.Contains(value))
            throw new ArgumentException($"'{value}' is not a valid trip state.", nameof(value));
        Value = value;
    }

    /// <summary>Parameterless constructor required by EF Core; defaults to <see cref="Pending" />.</summary>
    public TripState() : this(Pending)
    {
    }

    /// <summary>Factory for the initial state of a newly prepared trip.</summary>
    public static TripState CreatePending() => new(Pending);

    /// <summary>Returns <c>true</c> when the trip has not started yet.</summary>
    public bool IsPending() => Value == Pending;

    /// <summary>Returns <c>true</c> when the trip is currently in progress.</summary>
    public bool IsInProgress() => Value == InProgress;

    /// <summary>Returns <c>true</c> when the trip has been completed.</summary>
    public bool IsCompleted() => Value == Completed;

    /// <inheritdoc />
    public override string ToString() => Value;
}