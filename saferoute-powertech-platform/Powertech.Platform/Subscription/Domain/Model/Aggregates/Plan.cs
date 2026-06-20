

using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Subscription.Domain.Model.Commands;
using Powertech.Platform.Subscription.Domain.Model.ValueObjects;

namespace Powertech.Platform.Subscription.Domain.Model.Aggregates;

public class Plan
{
    protected Plan()
    {
        Id = new PlanId();
        Tier = new PlanTier();
        RouteQuota = new RouteQuota();
        DriverQuota = new DriverQuota();
    }

    public Plan(PlanTier tier, RouteQuota routeQuota, DriverQuota driverQuota, decimal price)
    {
        if (price < 0) throw new ArgumentException("Plan price cannot be negative.", nameof(price));
        Id = PlanId.New();
        Tier = tier;
        RouteQuota = routeQuota;
        DriverQuota = driverQuota;
        Price = price;
    }

    public Plan(CreatePlanCommand command)
        : this(new PlanTier(command.PlanTier), new RouteQuota(command.MaxRoutes),
            new DriverQuota(command.MaxDrivers), command.Price)
    {
    }

    public PlanId Id { get; private set; }

    public PlanTier Tier { get; private set; }

    public RouteQuota RouteQuota { get; private set; }

    public DriverQuota DriverQuota { get; private set; }

    public decimal Price { get; private set; }

    public string GetPlanName() => Tier.ToString();

    public int GetRouteLimit() => RouteQuota.GetMaxRoutes();

    public int GetDriverLimit() => DriverQuota.GetMaxDrivers();

    public bool IsWithinRouteQuota(int current) => RouteQuota.IsWithinLimit(current);

    public bool IsWithinDriverQuota(int current) => DriverQuota.IsWithinLimit(current);
}
