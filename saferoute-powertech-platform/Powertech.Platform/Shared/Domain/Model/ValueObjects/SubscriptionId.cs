namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a subscription across bounded contexts.
/// </summary>
/// <param name="Identifier">The unique identifier of the subscription.</param>
public record SubscriptionId(Guid Identifier)
{
    public SubscriptionId() : this(Guid.NewGuid())
    {
    }

    public static SubscriptionId New() => new(Guid.NewGuid());

    public override string ToString() => Identifier.ToString();
}