namespace Powertech.Platform.Iam.Interfaces.Rest.Resources;

public record UserResource(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string RoleTier,
    Guid? OrganizationId);
