using Safer_Route_Platform.Shared.Domain.Model.ValueObjects;
using Safer_Route_Platform.Subscription.Domain.Model.Commands;
using Safer_Route_Platform.Subscription.Domain.Model.ValueObjects;

namespace Safer_Route_Platform.Subscription.Domain.Model.Aggregates;

public class Subscription
{
    protected Subscription()
    {
        Id = new SubscriptionId();
        OrganizationId = new OrganizationId();
        PlanId = new PlanId();
        State = new SubscriptionState();
    }

    public Subscription(OrganizationId organizationId, PlanId planId, DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("Subscription end date must be after the start date.", nameof(endDate));
        Id = SubscriptionId.New();
        OrganizationId = organizationId;
        PlanId = planId;
        State = new SubscriptionState(SubscriptionState.Active);
        StartDate = startDate;
        EndDate = endDate;
    }

    public Subscription(CreateSubscriptionCommand command)
        : this(new OrganizationId(command.OrganizationId), new PlanId(command.PlanId), command.StartDate,
            command.EndDate)
    {
    }

    public SubscriptionId Id { get; private set; }

    public OrganizationId OrganizationId { get; private set; }

    public PlanId PlanId { get; private set; }

    public SubscriptionState State { get; private set; }

    public DateTimeOffset StartDate { get; private set; }

    public DateTimeOffset EndDate { get; private set; }

    public void Activate() => State = new SubscriptionState(SubscriptionState.Active);

    public void Upgrade(PlanId planId) => PlanId = planId;

    public void Cancel() => State = new SubscriptionState(SubscriptionState.Cancelled);

    public bool IsActive() => State.IsActive();

    public PlanId GetPlanId() => PlanId;

    public int GetRemainingDays()
    {
        var remaining = EndDate - DateTimeOffset.UtcNow;
        return remaining.TotalDays <= 0 ? 0 : (int)Math.Ceiling(remaining.TotalDays);
    }
}
