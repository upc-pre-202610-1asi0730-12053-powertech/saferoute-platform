namespace Powertech.Platform.Subscription.Interfaces.Rest.Resources;

public record CreatePlanResource(string PlanTier, int MaxRoutes, int MaxDrivers, decimal Price);
