namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Output resource representing the driver/children assignment of a route.</summary>
/// <param name="Id">The assignment identifier (Guid as string).</param>
/// <param name="DriverId">The assigned driver identifier (Guid as string).</param>
/// <param name="ChildIds">The identifiers of the children assigned to the route.</param>
public record AssignmentResource(
    string Id,
    string DriverId,
    IReadOnlyList<string> ChildIds);
