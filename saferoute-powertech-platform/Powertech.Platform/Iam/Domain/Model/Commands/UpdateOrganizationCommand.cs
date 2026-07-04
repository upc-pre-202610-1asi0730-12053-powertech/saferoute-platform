namespace Powertech.Platform.Iam.Domain.Model.Commands;

public record UpdateOrganizationCommand(Guid OrganizationId, string Name);