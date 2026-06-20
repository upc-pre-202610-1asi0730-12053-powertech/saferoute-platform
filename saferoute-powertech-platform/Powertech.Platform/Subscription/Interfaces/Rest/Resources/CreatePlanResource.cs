namespace Safer_Route_Platform.Subscription.Interfaces.Rest.Resources;

public record CreatePlanResource(string PlanTier, int MaxRoutes, int MaxDrivers, decimal Price);
