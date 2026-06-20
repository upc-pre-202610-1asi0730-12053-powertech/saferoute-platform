namespace Powertech.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Shared value object that identifies a subscription plan across bounded contexts.
/// </summary>
/// <param name="Identifier">The unique identifier of the plan.</param>
public record PlanId(Guid Identifier)
{
    public PlanId() : this(Guid.NewGuid())
    {
    }

    public static PlanId New() => new(Guid.NewGuid());

    public override string ToString() => Identifier.ToString();
}
