namespace Powertech.Platform.Stakeholder.Domain.Model.Commands;

public record CreateDriverCommand(
    Guid OrganizationId,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber);
