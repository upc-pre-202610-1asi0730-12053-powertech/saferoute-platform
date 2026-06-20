namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;
/// <summary>Input resource to assign a vehicle to a route.</summary>
/// <param name="Plate">The vehicle license plate.</param>
/// <param name="Model">The vehicle model.</param>
/// <param name="Brand">The vehicle brand.</param>
/// <param name="Capacity">The seating capacity.</param>
public record AssignVehicleResource(string Plate, string Model, string Brand, int Capacity);
