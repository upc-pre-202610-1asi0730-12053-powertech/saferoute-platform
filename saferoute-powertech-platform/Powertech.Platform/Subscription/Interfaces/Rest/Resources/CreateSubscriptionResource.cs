namespace Powertech.Platform.Subscription.Interfaces.Rest.Resources;

public record CreateSubscriptionResource(Guid OrganizationId, Guid PlanId, DateTimeOffset StartDate, DateTimeOffset EndDate);
