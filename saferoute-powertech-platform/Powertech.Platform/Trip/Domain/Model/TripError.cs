namespace Powertech.Platform.Trip.Domain.Model;

/// <summary>
///     Enumerates the failure cases of the Trip bounded context.
/// </summary>
/// <remarks>
///     Used by the Result pattern to convey a typed, localizable error from the application layer
///     to the interface layer, where it is mapped to an HTTP status code and a problem detail.
/// </remarks>
public enum TripError
{
    /// <summary>No error.</summary>
    None,

    /// <summary>The requested trip was not found.</summary>
    TripNotFound,

    /// <summary>The trip is in a state that does not allow the requested transition.</summary>
    InvalidTripState,

    /// <summary>The provided trip data is invalid.</summary>
    InvalidTripData,

    /// <summary>The operation was cancelled.</summary>
    OperationCancelled,

    /// <summary>A database error occurred.</summary>
    DatabaseError,

    /// <summary>An unexpected internal error occurred.</summary>
    InternalServerError
}
