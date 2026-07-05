namespace Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Entities;

/// <summary>
///     Persistence model for the standalone vehicle catalog consumed by the web app compatibility
///     endpoints.
/// </summary>
public class VehicleCatalogItem
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public string Plate { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string Status { get; set; } = "ACTIVE";

    public void Update(string plate, string model, int capacity, string status)
    {
        Plate = plate;
        Model = model;
        Capacity = capacity;
        Status = status;
    }
}
