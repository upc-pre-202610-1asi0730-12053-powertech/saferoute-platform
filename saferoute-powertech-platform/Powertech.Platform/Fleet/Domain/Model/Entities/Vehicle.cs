using Safer_Route_Platform.Fleet.Domain.Model.ValueObjects;
using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Fleet.Domain.Model.Entities;

/// <summary>
///     Entity representing the vehicle assigned to operate a route.
/// </summary>
/// <remarks>
///     <c>Vehicle</c> is a child entity of the <see cref="Aggregates.Route" /> aggregate, set when a
///     vehicle is selected for the route during its setup.
/// </remarks>
public class Vehicle
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Vehicle()
    {
        Id = new VehicleId();
        OrganizationId = new OrganizationId();
        Plate = string.Empty;
        Model = string.Empty;
        Brand = string.Empty;
    }

    /// <summary>Creates a new vehicle.</summary>
    /// <param name="organizationId">The organization that owns the vehicle.</param>
    /// <param name="plate">The license plate.</param>
    /// <param name="model">The vehicle model.</param>
    /// <param name="brand">The vehicle brand.</param>
    /// <param name="capacity">The seating capacity; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is not positive.</exception>
    public Vehicle(OrganizationId organizationId, string plate, string model, string brand, int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Vehicle capacity must be positive.");

        Id = VehicleId.New();
        OrganizationId = organizationId;
        Plate = plate;
        Model = model;
        Brand = brand;
        Capacity = capacity;
    }

    /// <summary>Local identity of the vehicle.</summary>
    public VehicleId Id { get; private set; }

    /// <summary>The organization that owns the vehicle.</summary>
    public OrganizationId OrganizationId { get; private set; }

    /// <summary>The license plate.</summary>
    public string Plate { get; private set; }

    /// <summary>The vehicle model.</summary>
    public string Model { get; private set; }

    /// <summary>The vehicle brand.</summary>
    public string Brand { get; private set; }

    /// <summary>The seating capacity.</summary>
    public int Capacity { get; private set; }

    /// <summary>Returns <c>true</c> when the vehicle can be used (has positive capacity).</summary>
    public bool IsAvailable() => Capacity > 0;

    /// <summary>Returns the license plate.</summary>
    public string GetPlate() => Plate;

    /// <summary>Returns the seating capacity.</summary>
    public int GetCapacity() => Capacity;

    /// <summary>Updates the descriptive details of the vehicle.</summary>
    /// <param name="model">The new model.</param>
    /// <param name="brand">The new brand.</param>
    public void UpdateDetails(string model, string brand)
    {
        Model = model;
        Brand = brand;
    }
}
