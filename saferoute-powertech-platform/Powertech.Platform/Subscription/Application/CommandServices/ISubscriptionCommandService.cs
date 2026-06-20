using Safer_Route_Platform.Shared.Application.Model;
using Safer_Route_Platform.Subscription.Domain.Model.Aggregates;
using Safer_Route_Platform.Subscription.Domain.Model.Commands;
using SubscriptionAggregate = Safer_Route_Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Safer_Route_Platform.Subscription.Application.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Result<Plan>> Handle(CreatePlanCommand command, CancellationToken cancellationToken);

    Task<Result<SubscriptionAggregate>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken);

    Task<Result<SubscriptionAggregate>> Handle(ActivateSubscriptionCommand command, CancellationToken cancellationToken);

    Task<Result<SubscriptionAggregate>> Handle(UpgradeSubscriptionCommand command, CancellationToken cancellationToken);

    Task<Result<SubscriptionAggregate>> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken);
}
