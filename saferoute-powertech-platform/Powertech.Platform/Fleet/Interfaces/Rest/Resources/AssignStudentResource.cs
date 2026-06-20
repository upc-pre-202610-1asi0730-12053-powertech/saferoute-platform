namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Input resource to assign a child (student) to a route.</summary>
/// <param name="ChildId">The child identifier (Guid as string).</param>

public record AssignStudentResource(string ChildId);