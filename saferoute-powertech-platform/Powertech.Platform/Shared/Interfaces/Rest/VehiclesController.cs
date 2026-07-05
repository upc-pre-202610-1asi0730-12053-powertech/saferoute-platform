using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Entities;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Powertech.Platform.Shared.Interfaces.Rest.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Powertech.Platform.Shared.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Vehicle compatibility endpoints used by the web application.")]
public class VehiclesController(
    AppDbContext context,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Create Vehicle", "Creates a vehicle record used by route planning views.",
        OperationId = "CreateVehicle")]
    [SwaggerResponse(201, "The vehicle was created.", typeof(VehicleResource))]
    [SwaggerResponse(400, "The vehicle data is invalid.")]
    public async Task<IActionResult> CreateVehicle(CreateVehicleResource resource, CancellationToken cancellationToken)
    {
        if (!IsValid(resource.OrganizationId, resource.Plate, resource.Model, resource.Capacity))
            return CreateProblem(StatusCodes.Status400BadRequest, VehicleEndpointError.InvalidVehicleData);

        var vehicle = new VehicleCatalogItem
        {
            Id = Guid.NewGuid(),
            OrganizationId = resource.OrganizationId,
            Plate = resource.Plate.Trim(),
            Model = resource.Model.Trim(),
            Capacity = resource.Capacity,
            Status = NormalizeStatus(resource.Status)
        };

        await context.Set<VehicleCatalogItem>().AddAsync(vehicle, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetVehicleById), new { vehicleId = vehicle.Id }, ToResource(vehicle));
    }

    [HttpGet("{vehicleId:guid}")]
    [SwaggerOperation("Get Vehicle by Id", "Gets a vehicle record by id.", OperationId = "GetVehicleById")]
    [SwaggerResponse(200, "The vehicle was found.", typeof(VehicleResource))]
    [SwaggerResponse(404, "The vehicle was not found.")]
    public async Task<IActionResult> GetVehicleById(Guid vehicleId, CancellationToken cancellationToken)
    {
        var vehicle = await FindVehicleAsync(vehicleId, cancellationToken);
        return vehicle is null
            ? CreateProblem(StatusCodes.Status404NotFound, VehicleEndpointError.VehicleNotFound)
            : Ok(vehicle);
    }

    [HttpGet]
    [SwaggerOperation("Get Vehicles", "Gets all vehicle records.", OperationId = "GetVehicles")]
    [SwaggerResponse(200, "The vehicles were found.", typeof(IEnumerable<VehicleResource>))]
    public async Task<IActionResult> GetVehicles(CancellationToken cancellationToken)
    {
        var vehicles = await context.Set<VehicleCatalogItem>()
            .OrderBy(vehicle => vehicle.Plate)
            .Select(vehicle => ToResource(vehicle))
            .ToListAsync(cancellationToken);

        return Ok(vehicles);
    }

    [HttpPut("{vehicleId:guid}")]
    [SwaggerOperation("Update Vehicle", "Updates a vehicle record.", OperationId = "UpdateVehicle")]
    [SwaggerResponse(200, "The vehicle was updated.", typeof(VehicleResource))]
    [SwaggerResponse(400, "The vehicle data is invalid.")]
    [SwaggerResponse(404, "The vehicle was not found.")]
    public async Task<IActionResult> UpdateVehicle(Guid vehicleId, UpdateVehicleResource resource,
        CancellationToken cancellationToken)
    {
        if (!IsValid(resource.Plate, resource.Model, resource.Capacity))
            return CreateProblem(StatusCodes.Status400BadRequest, VehicleEndpointError.InvalidVehicleData);

        var vehicle = await context.Set<VehicleCatalogItem>()
            .FirstOrDefaultAsync(item => item.Id == vehicleId, cancellationToken);
        if (vehicle is null)
            return CreateProblem(StatusCodes.Status404NotFound, VehicleEndpointError.VehicleNotFound);

        vehicle.Update(resource.Plate.Trim(), resource.Model.Trim(), resource.Capacity, NormalizeStatus(resource.Status));
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ToResource(vehicle));
    }

    [HttpDelete("{vehicleId:guid}")]
    [SwaggerOperation("Delete Vehicle", "Deletes a vehicle record.", OperationId = "DeleteVehicle")]
    [SwaggerResponse(200, "The vehicle was deleted.", typeof(VehicleResource))]
    [SwaggerResponse(404, "The vehicle was not found.")]
    public async Task<IActionResult> DeleteVehicle(Guid vehicleId, CancellationToken cancellationToken)
    {
        var vehicle = await FindVehicleAsync(vehicleId, cancellationToken);
        if (vehicle is null)
            return CreateProblem(StatusCodes.Status404NotFound, VehicleEndpointError.VehicleNotFound);

        var entity = await context.Set<VehicleCatalogItem>()
            .FirstAsync(item => item.Id == vehicleId, cancellationToken);

        context.Set<VehicleCatalogItem>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(vehicle);
    }

    private async Task<VehicleResource?> FindVehicleAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        var vehicle = await context.Set<VehicleCatalogItem>()
            .FirstOrDefaultAsync(item => item.Id == vehicleId, cancellationToken);
        return vehicle is null ? null : ToResource(vehicle);
    }

    private IActionResult CreateProblem(int statusCode, VehicleEndpointError error) =>
        problemDetailsFactory.CreateProblemDetails(this, statusCode, error, error.ToString());

    private static VehicleResource ToResource(VehicleCatalogItem vehicle) =>
        new(vehicle.Id, vehicle.OrganizationId, vehicle.Plate, vehicle.Model, vehicle.Capacity, vehicle.Status);

    private static bool IsValid(Guid organizationId, string? plate, string? model, int capacity) =>
        organizationId != Guid.Empty &&
        IsValid(plate, model, capacity);

    private static bool IsValid(string? plate, string? model, int capacity) =>
        !string.IsNullOrWhiteSpace(plate) &&
        !string.IsNullOrWhiteSpace(model) &&
        capacity > 0;

    private static string NormalizeStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? "ACTIVE" : status.Trim().ToUpperInvariant();

    private enum VehicleEndpointError
    {
        InvalidVehicleData,
        VehicleNotFound
    }
}
