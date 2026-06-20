namespace Powertech.Platform.Subscription.Domain.Model.Commands;

public record UpgradeSubscriptionCommand(Guid SubscriptionId, Guid PlanId);
