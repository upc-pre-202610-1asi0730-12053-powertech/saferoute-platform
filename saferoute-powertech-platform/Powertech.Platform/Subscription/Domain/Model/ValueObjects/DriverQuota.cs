namespace Safer_Route_Platform.Subscription.Domain.Model.ValueObjects;

public record DriverQuota
{
    public DriverQuota() : this(0, false)
    {
    }

    public DriverQuota(int maxDrivers) : this(maxDrivers, true)
    {
    }

    public DriverQuota(int maxDrivers, bool validate)
    {
        if (validate && maxDrivers < 0)
            throw new ArgumentException("Driver quota cannot be negative.", nameof(maxDrivers));
        MaxDrivers = maxDrivers;
    }

    public int MaxDrivers { get; init; }

    public bool IsWithinLimit(int current) => current <= MaxDrivers;

    public int GetMaxDrivers() => MaxDrivers;
}
