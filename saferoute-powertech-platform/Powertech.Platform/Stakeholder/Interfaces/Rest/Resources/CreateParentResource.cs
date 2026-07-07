namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record CreateParentResource(
    Guid OrganizationId,
    Guid? UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? Password = null);
