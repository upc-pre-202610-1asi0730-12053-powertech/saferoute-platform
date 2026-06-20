using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Subscription.Application.CommandServices;
using Powertech.Platform.Subscription.Domain.Model;
using Powertech.Platform.Subscription.Domain.Model.Aggregates;
using Powertech.Platform.Subscription.Domain.Model.Commands;
using Powertech.Platform.Subscription.Domain.Repositories;
using SubscriptionAggregate = Powertech.Platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace Powertech.Platform.Subscription.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    IPlanRepository planRepository,
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork) : ISubscriptionCommandService
{
    public async Task<Result<Plan>> Handle(CreatePlanCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var plan = new Plan(command);
            await planRepository.AddAsync(plan, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Plan>.Success(plan);
        }
        catch (ArgumentException ex)
        {
            return Result<Plan>.Failure(SubscriptionError.InvalidSubscriptionData, ex.Message);
        }
    }

    public async Task<Result<SubscriptionAggregate>> Handle(CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await planRepository.FindByPlanIdAsync(new PlanId(command.PlanId), cancellationToken);
            if (plan is null)
                return Result<SubscriptionAggregate>.Failure(SubscriptionError.PlanNotFound, "Plan was not found.");
            var subscription = new SubscriptionAggregate(command);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<SubscriptionAggregate>.Success(subscription);
        }
        catch (ArgumentException ex)
        {
            return Result<SubscriptionAggregate>.Failure(SubscriptionError.InvalidSubscriptionData, ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result<SubscriptionAggregate>.Failure(SubscriptionError.DatabaseError, "Database error.");
        }
    }

    public Task<Result<SubscriptionAggregate>> Handle(ActivateSubscriptionCommand command,
        CancellationToken cancellationToken) =>
        MutateAsync(command.SubscriptionId, subscription => subscription.Activate(), cancellationToken);

    public Task<Result<SubscriptionAggregate>> Handle(UpgradeSubscriptionCommand command,
        CancellationToken cancellationToken) =>
        MutateAsync(command.SubscriptionId, subscription => subscription.Upgrade(new PlanId(command.PlanId)),
            cancellationToken);

    public Task<Result<SubscriptionAggregate>> Handle(CancelSubscriptionCommand command,
        CancellationToken cancellationToken) =>
        MutateAsync(command.SubscriptionId, subscription => subscription.Cancel(), cancellationToken);

    private async Task<Result<SubscriptionAggregate>> MutateAsync(Guid subscriptionId,
        Action<SubscriptionAggregate> mutation, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindBySubscriptionIdAsync(new SubscriptionId(subscriptionId),
            cancellationToken);
        if (subscription is null)
            return Result<SubscriptionAggregate>.Failure(SubscriptionError.SubscriptionNotFound,
                "Subscription was not found.");
        mutation(subscription);
        subscriptionRepository.Update(subscription);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<SubscriptionAggregate>.Success(subscription);
    }
}
