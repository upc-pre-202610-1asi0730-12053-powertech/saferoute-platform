using System.Data;
using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Fleet.Domain.Model;
using Powertech.Platform.Fleet.Interfaces.Rest.Resources;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Powertech.Platform.Fleet.Interfaces.Rest;

[ApiController]
[Route("api/v1/routes")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Fleet route compatibility endpoints.")]
public class RouteCompatibilityController(
    AppDbContext context,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPut("{routeId:guid}")]
    [SwaggerOperation("Update Route", "Updates a route record and its current configuration.",
        OperationId = "UpdateRouteCompatibility")]
    public async Task<IActionResult> UpdateRoute(Guid routeId, UpdateRouteResource resource,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resource.Name))
            return CreateProblem(StatusCodes.Status400BadRequest, FleetError.InvalidRouteData);

        var connection = await OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var currentState = await ReadScalarAsync<string?>(
            connection,
            transaction,
            "SELECT state FROM routes WHERE id = @id LIMIT 1;",
            cancellationToken,
            ("@id", routeId.ToString()));

        if (currentState is null)
            return CreateProblem(StatusCodes.Status404NotFound, FleetError.RouteNotFound);

        var state = NormalizeState(resource.Status ?? resource.RouteState ?? currentState);
        var departureTime = NormalizeDepartureTime(resource.ScheduledStartTime ?? resource.DepartureTime);
        var serviceDays = resource.ServiceDays is { Count: > 0 }
            ? string.Join(',', resource.ServiceDays.Select(day => day.Trim().ToUpperInvariant()).Where(day => day.Length > 0))
            : "MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY";

        await ExecuteAsync(connection, transaction,
            """
            UPDATE routes
            SET name = @name,
                state = @state,
                departure_time = @departure_time,
                service_days = @service_days
            WHERE id = @id;
            """,
            cancellationToken,
            ("@id", routeId.ToString()),
            ("@name", resource.Name.Trim()),
            ("@state", state),
            ("@departure_time", (object?)departureTime ?? DBNull.Value),
            ("@service_days", serviceDays));

        await ExecuteAsync(connection, transaction, "DELETE FROM stops WHERE route_id = @id;", cancellationToken,
            ("@id", routeId.ToString()));
        await ExecuteAsync(connection, transaction, "DELETE FROM route_vehicles WHERE route_id = @id;", cancellationToken,
            ("@id", routeId.ToString()));
        await ExecuteAsync(connection, transaction, "DELETE FROM route_assignments WHERE route_id = @id;",
            cancellationToken, ("@id", routeId.ToString()));

        var waypoints = resource.Waypoints ?? [];
        for (var index = 0; index < waypoints.Count; index++)
        {
            var waypoint = waypoints[index];
            await ExecuteAsync(connection, transaction,
                """
                INSERT INTO stops (id, name, latitude, longitude, `order`, route_id)
                VALUES (@id, @name, @latitude, @longitude, @order, @route_id);
                """,
                cancellationToken,
                ("@id", ParseGuidOrNew(waypoint.Id).ToString()),
                ("@name", string.IsNullOrWhiteSpace(waypoint.Name) ? $"Parada {index + 1}" : waypoint.Name.Trim()),
                ("@latitude", waypoint.Latitude ?? waypoint.Lat ?? 0),
                ("@longitude", waypoint.Longitude ?? waypoint.Lng ?? 0),
                ("@order", waypoint.Order ?? index + 1),
                ("@route_id", routeId.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(resource.VehiclePlate))
        {
            await ExecuteAsync(connection, transaction,
                """
                INSERT INTO route_vehicles (id, organization_id, plate, model, brand, capacity, route_id)
                SELECT @id, organization_id, @plate, @model, @brand, @capacity, id
                FROM routes
                WHERE id = @route_id;
                """,
                cancellationToken,
                ("@id", Guid.NewGuid().ToString()),
                ("@plate", resource.VehiclePlate.Trim()),
                ("@model", string.IsNullOrWhiteSpace(resource.VehicleModel) ? "Vehicle" : resource.VehicleModel.Trim()),
                ("@brand", string.IsNullOrWhiteSpace(resource.VehicleBrand) ? "SafeRoute" : resource.VehicleBrand.Trim()),
                ("@capacity", Math.Max(1, resource.VehicleCapacity ?? 1)),
                ("@route_id", routeId.ToString()));
        }

        if (Guid.TryParse(resource.DriverId, out var driverId))
        {
            var childIds = resource.StudentIds is { Count: > 0 }
                ? string.Join(',', resource.StudentIds.Where(id => Guid.TryParse(id, out _)))
                : string.Empty;

            await ExecuteAsync(connection, transaction,
                """
                INSERT INTO route_assignments (id, driver_id, child_ids, route_id)
                VALUES (@id, @driver_id, @child_ids, @route_id);
                """,
                cancellationToken,
                ("@id", Guid.NewGuid().ToString()),
                ("@driver_id", driverId.ToString()),
                ("@child_ids", childIds),
                ("@route_id", routeId.ToString()));
        }

        await transaction.CommitAsync(cancellationToken);
        return Ok(await ReadRouteResourceAsync(routeId, resource, cancellationToken));
    }

    [HttpDelete("{routeId:guid}")]
    [SwaggerOperation("Delete Route", "Deletes a route and dependent records used by the web application.",
        OperationId = "DeleteRouteCompatibility")]
    public async Task<IActionResult> DeleteRoute(Guid routeId, CancellationToken cancellationToken)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var exists = await ReadScalarAsync<long>(connection, transaction,
            "SELECT COUNT(*) FROM routes WHERE id = @id;",
            cancellationToken,
            ("@id", routeId.ToString()));

        if (exists == 0)
            return CreateProblem(StatusCodes.Status404NotFound, FleetError.RouteNotFound);

        await ExecuteAsync(connection, transaction,
            "DELETE FROM notifications WHERE trip_id IN (SELECT id FROM trips WHERE route_id = @id);",
            cancellationToken,
            ("@id", routeId.ToString()));
        await ExecuteAsync(connection, transaction, "DELETE FROM trips WHERE route_id = @id;", cancellationToken,
            ("@id", routeId.ToString()));
        await ExecuteAsync(connection, transaction, "DELETE FROM routes WHERE id = @id;", cancellationToken,
            ("@id", routeId.ToString()));

        await transaction.CommitAsync(cancellationToken);
        return NoContent();
    }

    private async Task<object> ReadRouteResourceAsync(Guid routeId, UpdateRouteResource fallback,
        CancellationToken cancellationToken)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT id, organization_id, name, state, departure_time, service_days
                              FROM routes
                              WHERE id = @id
                              LIMIT 1;
                              """;
        AddParameter(command, "@id", routeId.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return fallback;

        var serviceDays = ReadNullableString(reader, "service_days")?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        return new
        {
            id = ReadGuid(reader, "id").ToString(),
            organizationId = ReadGuid(reader, "organization_id").ToString(),
            name = ReadString(reader, "name"),
            state = ReadString(reader, "state"),
            departureTime = ReadNullableString(reader, "departure_time"),
            serviceDays,
            vehicle = string.IsNullOrWhiteSpace(fallback.VehiclePlate)
                ? null
                : new
                {
                    id = fallback.VehicleId,
                    plate = fallback.VehiclePlate,
                    model = fallback.VehicleModel,
                    brand = fallback.VehicleBrand ?? "SafeRoute",
                    capacity = fallback.VehicleCapacity ?? 1
                },
            assignment = string.IsNullOrWhiteSpace(fallback.DriverId)
                ? null
                : new
                {
                    id = Guid.NewGuid().ToString(),
                    driverId = fallback.DriverId,
                    childIds = fallback.StudentIds ?? []
                },
            stops = (fallback.Waypoints ?? []).Select((waypoint, index) => new
            {
                id = waypoint.Id ?? Guid.NewGuid().ToString(),
                name = waypoint.Name ?? $"Parada {index + 1}",
                latitude = waypoint.Latitude ?? waypoint.Lat ?? 0,
                longitude = waypoint.Longitude ?? waypoint.Lng ?? 0,
                order = waypoint.Order ?? index + 1
            })
        };
    }

    private async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        return connection;
    }

    private static async Task ExecuteAsync(DbConnection connection, DbTransaction transaction, string sql,
        CancellationToken cancellationToken, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
            AddParameter(command, parameter.Name, parameter.Value ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<T?> ReadScalarAsync<T>(DbConnection connection, DbTransaction transaction, string sql,
        CancellationToken cancellationToken, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
            AddParameter(command, parameter.Name, parameter.Value ?? DBNull.Value);

        var value = await command.ExecuteScalarAsync(cancellationToken);
        if (value is null || value is DBNull) return default;
        return (T)Convert.ChangeType(value, typeof(T));
    }

    private IActionResult CreateProblem(int statusCode, FleetError error) =>
        problemDetailsFactory.CreateProblemDetails(this, statusCode, error, error.ToString());

    private static string NormalizeState(string? state)
    {
        var normalized = string.IsNullOrWhiteSpace(state) ? "DRAFT" : state.Trim().ToUpperInvariant();
        return normalized is "ACTIVE" or "INACTIVE" or "DRAFT" ? normalized : "DRAFT";
    }

    private static string? NormalizeDepartureTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return TimeOnly.TryParse(value, out var time) ? time.ToString("HH:mm") : value.Trim();
    }

    private static Guid ParseGuidOrNew(string? value) =>
        Guid.TryParse(value, out var id) ? id : Guid.NewGuid();

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

    private static string? ReadNullableString(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
