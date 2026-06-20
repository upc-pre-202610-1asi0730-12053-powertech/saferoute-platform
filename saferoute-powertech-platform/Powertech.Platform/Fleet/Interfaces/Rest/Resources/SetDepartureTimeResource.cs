namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Input resource to set the departure time of a route.</summary>
/// <param name="DepartureTime">The departure time in HH:mm format.</param>
public record SetDepartureTimeResource(string DepartureTime);
