namespace Powertech.Platform.Subscription.Domain.Model.ValueObjects;

public record RouteQuota
{
    public RouteQuota() : this(0, false)
    {
    }

    public RouteQuota(int maxRoutes) : this(maxRoutes, true)
    {
    }

    public RouteQuota(int maxRoutes, bool validate)
    {
        if (validate && maxRoutes < 0)
            throw new ArgumentException("Route quota cannot be negative.", nameof(maxRoutes));
        MaxRoutes = maxRoutes;
    }

    public int MaxRoutes { get; init; }

    public bool IsWithinLimit(int current) => current <= MaxRoutes;

    public int GetMaxRoutes() => MaxRoutes;
}
