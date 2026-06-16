namespace Powertech.Platform.Trip.Domain.Model.Commands;

/// <summary>
///     Command issued by the driver to complete an in-progress trip.
/// </summary>
/// <remarks>Maps to the "Complete Trip" event-storming command, producing a <c>TripCompleted</c> event.</remarks>
/// <param name="TripId">The identifier of the trip to complete.</param>
public record CompleteTripCommand(Guid TripId);
