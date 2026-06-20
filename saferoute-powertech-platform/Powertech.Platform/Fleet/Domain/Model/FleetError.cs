namespace Powertech.Platform.Fleet.Domain.Model;

/// <summary>
///     Enumerates the failure cases of the Fleet bounded context.
/// </summary>
/// <remarks>
///     Conveyed through the Result pattern from the application layer to the interface layer, where
///     it is mapped to an HTTP status code and an RFC 7807 problem detail.
/// </remarks>
public enum FleetError
{
    /// <summary>No error.</summary>
    None,

    /// <summary>The requested route was not found.</summary>
    RouteNotFound,

    /// <summary>The route is in a state that does not allow the requested operation.</summary>
    InvalidRouteState,

    /// <summary>The provided route data is invalid.</summary>
    InvalidRouteData,

    /// <summary>The operation was cancelled.</summary>
    OperationCancelled,

    /// <summary>A database error occurred.</summary>
    DatabaseError,

    /// <summary>An unexpected internal error occurred.</summary>
    InternalServerError
}