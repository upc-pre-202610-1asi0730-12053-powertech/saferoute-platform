namespace Powertech.Platform.Trip.Interfaces.Rest.Resources;

/// <summary>
///     Input resource to record a child's boarding status during a trip.
/// </summary>
/// <param name="ChildId">The child whose boarding is recorded (Guid as string).</param>
/// <param name="BoardingState">The boarding state (BOARDED, MISSING or OMITTED).</param>
public record SetBoardingStatusResource(
    string ChildId,
    string BoardingState);