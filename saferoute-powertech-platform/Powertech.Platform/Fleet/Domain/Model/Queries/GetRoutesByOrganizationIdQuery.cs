namespace Powertech.Platform.Fleet.Domain.Model.Queries;


/// <summary>Query to retrieve all routes that belong to a given organization.</summary>
/// <param name="OrganizationId">The organization identifier to filter routes by.</param>


public record GetRoutesByOrganizationIdQuery(Guid OrganizationId);