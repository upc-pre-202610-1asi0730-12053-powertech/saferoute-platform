namespace Powertech.Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to choose and assign a vehicle to a route.
/// </summary>
/// <remarks>Maps to the "Choose Vehicle" event-storming command, producing a <c>VehicleAssignedToRoute</c> event.</remarks>
/// <param name="RouteId">The route to assign the vehicle to.</param>
/// <param name="Plate">The vehicle license plate.</param>
/// <param name="Model">The vehicle model.</param>
/// <param name="Brand">The vehicle brand.</param>
/// <param name="Capacity">The seating capacity.</param>
public record AssignVehicleCommand(Guid RouteId, string Plate, string Model, string Brand, int Capacity);
