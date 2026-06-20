namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;

/// <summary>Input resource to define (create) a new route.</summary>
/// <param name="OrganizationId">The owning organization identifier (Guid as string).</param>
/// <param name="Name">The route name.</param>
public record CreateRouteResource(string OrganizationId, string Name);
