namespace Powertech.Platform.Subscription.Domain.Model.ValueObjects;

public record SubscriptionState
{
    public const string Active = "ACTIVE";
    public const string Expired = "EXPIRED";
    public const string Cancelled = "CANCELLED";

    public SubscriptionState() : this(Active, false)
    {
    }

    public SubscriptionState(string value) : this(value, true)
    {
    }

    public SubscriptionState(string value, bool validate)
    {
        Value = value.Trim().ToUpperInvariant();
        if (validate && Value is not Active and not Expired and not Cancelled)
            throw new ArgumentException("Invalid subscription state.", nameof(value));
    }

    public string Value { get; init; } = Active;

    public bool IsActive() => Value == Active;

    public bool IsExpired() => Value == Expired;

    public bool IsCancelled() => Value == Cancelled;

    public override string ToString() => Value;
}
