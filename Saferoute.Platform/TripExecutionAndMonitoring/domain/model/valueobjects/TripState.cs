namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.valueobjects;

/// <summary>
/// Enumeration representing the lifecycle state of a Trip.
/// <para>
/// Drives the trip execution flow: a trip begins <see cref="InProgress"/>
/// when the driver starts the route and becomes <see cref="Completed"/> once the route is closed. 
/// A trip may also be <see cref="Cancelled"/> when aborted before completion.
/// </para>
/// </summary>
public enum TripState
{
    /// <summary>
    /// The trip is currently underway; coordinates and boarding are being tracked. Initial state.
    /// </summary>
    InProgress,

    /// <summary>
    /// The trip has been finished by the driver.
    /// </summary>
    Completed,

    /// <summary>
    /// The trip was aborted before completion.
    /// </summary>
    Cancelled
}