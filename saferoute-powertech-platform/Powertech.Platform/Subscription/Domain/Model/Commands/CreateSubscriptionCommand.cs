namespace Safer_Route_Platform.Subscription.Domain.Model.Commands;

public record CreateSubscriptionCommand(Guid OrganizationId, Guid PlanId, DateTimeOffset StartDate, DateTimeOffset EndDate);
