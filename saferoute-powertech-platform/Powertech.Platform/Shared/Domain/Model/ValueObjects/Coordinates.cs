namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object representing a geographic location (latitude / longitude).
/// </summary>
/// <remarks>
///     Used by the <c>Fleet</c> context to locate a stop and, conceptually, by live-tracking
///     features in <c>Trip</c>. It enforces the valid WGS-84 ranges as an invariant so an
///     invalid coordinate can never exist in the domain.
/// </remarks>
/// <param name="Latitude">The latitude, between -90 and 90 degrees.</param>
/// <param name="Longitude">The longitude, between -180 and 180 degrees.</param>
public record Coordinates
{
    /// <summary>The latitude component, in decimal degrees.</summary>
    public double Latitude { get; init; }

    /// <summary>The longitude component, in decimal degrees.</summary>
    public double Longitude { get; init; }

    /// <summary>
    ///     Creates a new <see cref="Coordinates" /> instance, validating the WGS-84 ranges.
    /// </summary>
    /// <param name="latitude">Latitude between -90 and 90.</param>
    /// <param name="longitude">Longitude between -180 and 180.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a component is out of range.</exception>
    public Coordinates(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public Coordinates() : this(0, 0)
    {
    }

    /// <inheritdoc />
    public override string ToString() => $"{Latitude}, {Longitude}";
}
