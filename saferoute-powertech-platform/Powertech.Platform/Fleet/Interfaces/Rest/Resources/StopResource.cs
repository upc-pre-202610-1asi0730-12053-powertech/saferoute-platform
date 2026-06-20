namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Output resource representing a stop within a route.</summary>
/// <param name="Id">The stop identifier (Guid as string).</param>
/// <param name="Name">The stop name.</param>
/// <param name="Latitude">The stop latitude.</param>
/// <param name="Longitude">The stop longitude.</param>
/// <param name="Order">The 1-based position of the stop in the sequence.</param>
public record StopResource(
    string Id,
    string Name,
    double Latitude,
    double Longitude,
    int Order);
