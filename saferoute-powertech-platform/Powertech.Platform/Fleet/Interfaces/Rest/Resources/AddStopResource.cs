namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Input resource to append a stop to a route.</summary>
/// <param name="Name">The stop name.</param>
/// <param name="Latitude">The stop latitude.</param>
/// <param name="Longitude">The stop longitude.</param>
public record AddStopResource(string Name, double Latitude, double Longitude);
