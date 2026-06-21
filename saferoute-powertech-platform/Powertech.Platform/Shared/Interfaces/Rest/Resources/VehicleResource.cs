namespace Powertech.Platform.Shared.Interfaces.Rest.Resources;

public record VehicleResource(
    Guid Id,
    Guid OrganizationId,
    string Plate,
    string Model,
    int Capacity,
    string Status);
