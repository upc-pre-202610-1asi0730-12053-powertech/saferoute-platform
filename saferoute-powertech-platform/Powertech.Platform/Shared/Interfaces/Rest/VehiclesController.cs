using System.Data;
using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
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

        var vehicle = new VehicleResource(
            Guid.NewGuid(),
            resource.OrganizationId,
            resource.Plate.Trim(),
            resource.Model.Trim(),
            resource.Capacity,
            NormalizeStatus(resource.Status));

        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = """
                              INSERT INTO vehicles (id, organization_id, plate, model, capacity, status)
                              VALUES (@id, @organization_id, @plate, @model, @capacity, @status);
                              """;
        AddParameter(command, "@id", vehicle.Id.ToString());
        AddParameter(command, "@organization_id", vehicle.OrganizationId.ToString());
        AddParameter(command, "@plate", vehicle.Plate);
        AddParameter(command, "@model", vehicle.Model);
        AddParameter(command, "@capacity", vehicle.Capacity);
        AddParameter(command, "@status", vehicle.Status);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return CreatedAtAction(nameof(GetVehicleById), new { vehicleId = vehicle.Id }, vehicle);
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
        var vehicles = new List<VehicleResource>();
        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT id, organization_id, plate, model, capacity, status
                              FROM vehicles
                              ORDER BY plate;
                              """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            vehicles.Add(ToResource(reader));

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

        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = """
                              UPDATE vehicles
                              SET plate = @plate,
                                  model = @model,
                                  capacity = @capacity,
                                  status = @status
                              WHERE id = @id;
                              """;
        AddParameter(command, "@id", vehicleId.ToString());
        AddParameter(command, "@plate", resource.Plate.Trim());
        AddParameter(command, "@model", resource.Model.Trim());
        AddParameter(command, "@capacity", resource.Capacity);
        AddParameter(command, "@status", NormalizeStatus(resource.Status));

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        if (affectedRows == 0)
            return CreateProblem(StatusCodes.Status404NotFound, VehicleEndpointError.VehicleNotFound);

        var vehicle = await FindVehicleAsync(vehicleId, cancellationToken);
        return vehicle is null
            ? CreateProblem(StatusCodes.Status404NotFound, VehicleEndpointError.VehicleNotFound)
            : Ok(vehicle);
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

        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM vehicles WHERE id = @id;";
        AddParameter(command, "@id", vehicleId.ToString());

        await command.ExecuteNonQueryAsync(cancellationToken);
        return Ok(vehicle);
    }

    private async Task<VehicleResource?> FindVehicleAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT id, organization_id, plate, model, capacity, status
                              FROM vehicles
                              WHERE id = @id
                              LIMIT 1;
                              """;
        AddParameter(command, "@id", vehicleId.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ToResource(reader) : null;
    }

    private async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        return connection;
    }

    private IActionResult CreateProblem(int statusCode, VehicleEndpointError error) =>
        problemDetailsFactory.CreateProblemDetails(this, statusCode, error, error.ToString());

    private static VehicleResource ToResource(DbDataReader reader) =>
        new(
            ReadGuid(reader, "id"),
            ReadGuid(reader, "organization_id"),
            ReadString(reader, "plate"),
            ReadString(reader, "model"),
            reader.GetInt32(reader.GetOrdinal("capacity")),
            ReadString(reader, "status"));

    private static bool IsValid(Guid organizationId, string? plate, string? model, int capacity) =>
        organizationId != Guid.Empty &&
        IsValid(plate, model, capacity);

    private static bool IsValid(string? plate, string? model, int capacity) =>
        !string.IsNullOrWhiteSpace(plate) &&
        !string.IsNullOrWhiteSpace(model) &&
        capacity > 0;

    private static string NormalizeStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? "ACTIVE" : status.Trim().ToUpperInvariant();

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static Guid ReadGuid(DbDataReader reader, string name)
    {
        var value = reader.GetValue(reader.GetOrdinal(name));
        return value switch
        {
            Guid guid => guid,
            byte[] bytes when bytes.Length == 16 => new Guid(bytes),
            _ => Guid.Parse(value.ToString() ?? string.Empty)
        };
    }

    private static string ReadString(DbDataReader reader, string name) =>
        reader.GetString(reader.GetOrdinal(name));

    private enum VehicleEndpointError
    {
        InvalidVehicleData,
        VehicleNotFound
    }
}
