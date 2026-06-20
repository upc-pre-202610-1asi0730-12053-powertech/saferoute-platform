namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record DriverResource(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    bool Available);
