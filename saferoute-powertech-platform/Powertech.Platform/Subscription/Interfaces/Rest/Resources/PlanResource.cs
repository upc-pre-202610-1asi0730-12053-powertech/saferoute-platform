namespace Safer_Route_Platform.Subscription.Interfaces.Rest.Resources;

public record PlanResource(Guid Id, string PlanTier, int MaxRoutes, int MaxDrivers, decimal Price);
