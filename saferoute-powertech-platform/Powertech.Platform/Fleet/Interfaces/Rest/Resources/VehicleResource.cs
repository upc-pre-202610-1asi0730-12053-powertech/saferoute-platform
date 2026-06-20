namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Output resource representing the vehicle assigned to a route.</summary>
/// <param name="Id">The vehicle identifier (Guid as string).</param>
/// <param name="Plate">The license plate.</param>
/// <param name="Model">The vehicle model.</param>
/// <param name="Brand">The vehicle brand.</param>
/// <param name="Capacity">The seating capacity.</param>
public record VehicleResource(
    string Id,
    string Plate,
    string Model,
    string Brand,
    int Capacity);
