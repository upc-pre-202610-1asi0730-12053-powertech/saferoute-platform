namespace Powertech.Platform.Trip.Domain.Model.Commands;

/// <summary>
///     Command to record the boarding status of a child during an in-progress trip.
/// </summary>
/// <remarks>
///     Maps to the "Set Boarding Status" event-storming command (Student Boarded / Dropped Off).
/// </remarks>
/// <param name="TripId">The trip the boarding belongs to.</param>
/// <param name="ChildId">The child whose boarding is recorded.</param>
/// <param name="BoardingState">The boarding state value (BOARDED, MISSING or OMITTED).</param>
public record SetBoardingStatusCommand(
    Guid TripId,
    Guid ChildId,
    string BoardingState);
