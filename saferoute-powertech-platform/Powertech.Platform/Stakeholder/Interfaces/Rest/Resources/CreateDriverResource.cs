namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record CreateDriverResource(
    Guid OrganizationId,
    Guid? UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    string? Password = null);
