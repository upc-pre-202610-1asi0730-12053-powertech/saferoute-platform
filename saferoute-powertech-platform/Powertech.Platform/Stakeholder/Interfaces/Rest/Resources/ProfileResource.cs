namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record ProfileResource(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Phone,
    string Role,
    string? License,
    string Status);
