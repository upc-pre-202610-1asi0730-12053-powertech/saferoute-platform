namespace Powertech.Platform.Stakeholder.Domain.Model.Commands;

public record UpdateDriverCommand(
    Guid DriverId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    bool Available);
