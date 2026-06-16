namespace Powertech.Platform.Trip.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a single trip by its unique identifier.
/// </summary>
/// <param name="TripId">The identifier of the trip to retrieve.</param>
public record GetTripByIdQuery(Guid TripId);
