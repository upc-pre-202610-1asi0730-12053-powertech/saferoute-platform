namespace Powertech.Platform.Iam.Interfaces.Rest.Resources;

public record OrganizationResource(Guid Id, string Name, string Status, DateTimeOffset? CreatedAt);
