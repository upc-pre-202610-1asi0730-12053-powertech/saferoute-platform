namespace Powertech.Platform.Trip.Domain.Model.Commands;

/// <summary>
///     Command issued by the driver to start a prepared trip.
/// </summary>
/// <remarks>Maps to the "Start Trip" event-storming command, producing a <c>TripStarted</c> event.</remarks>
/// <param name="TripId">The identifier of the trip to start.</param>
public record StartTripCommand(Guid TripId);
