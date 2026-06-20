namespace Safer_Route_Platform.Subscription.Domain.Model;

public enum SubscriptionError
{
    InvalidSubscriptionData,
    PlanNotFound,
    SubscriptionNotFound,
    DatabaseError,
    InternalServerError
}
