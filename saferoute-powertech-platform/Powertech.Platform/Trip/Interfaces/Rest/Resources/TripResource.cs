namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Output resource representing a trip and its current state.
/// </summary>
/// <remarks>
///     This is the published contract consumed by the frontend Trip module. Identifiers are exposed
///     as strings (Guid), matching the shapes declared in the SafeRoute frontend domain models.
/// </remarks>
/// <param name="Id">The trip identifier (Guid as string).</param>
/// <param name="OrganizationId">The owning organization identifier.</param>
/// <param name="RouteId">The route the trip runs over.</param>
/// <param name="DriverId">The driver operating the trip.</param>
/// <param name="TripState">The current lifecycle state (PENDING, IN_PROGRESS, COMPLETED).</param>
/// <param name="StartTime">The moment the trip started, if any.</param>
/// <param name="EndTime">The moment the trip ended, if any.</param>
/// <param name="Attendances">The boarding attendance records.</param>
/// <param name="Incidents">The reported incidents.</param>
public record TripResource(
    string Id,
    string OrganizationId,
    string RouteId,
    string DriverId,
    string TripState,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    IReadOnlyList<AttendanceResource> Attendances,
    IReadOnlyList<IncidentResource> Incidents);
