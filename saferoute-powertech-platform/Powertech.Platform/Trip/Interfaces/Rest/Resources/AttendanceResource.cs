namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Output resource representing a boarding attendance record of a trip.
/// </summary>
/// <param name="Id">The attendance record identifier (Guid as string).</param>
/// <param name="ChildId">The identifier of the child (Guid as string).</param>
/// <param name="BoardingState">The boarding state (BOARDED, MISSING or OMITTED).</param>
/// <param name="BoardedAt">The boarding timestamp, when applicable.</param>
public record AttendanceResource(
    string Id,
    string ChildId,
    string BoardingState,
    DateTimeOffset? BoardedAt);
