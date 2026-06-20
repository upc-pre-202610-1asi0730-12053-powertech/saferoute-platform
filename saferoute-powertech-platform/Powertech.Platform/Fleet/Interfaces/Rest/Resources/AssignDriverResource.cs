namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Input resource to assign the operating driver to a route.</summary>
/// <param name="DriverId">The driver identifier (Guid as string).</param>
public record AssignDriverResource(string DriverId);