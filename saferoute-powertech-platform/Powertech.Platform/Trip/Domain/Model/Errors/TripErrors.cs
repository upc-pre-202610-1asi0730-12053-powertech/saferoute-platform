using Powertech.Platform.Shared.Domain.Model;

namespace Powertech.Platform.Trip.Domain.Model.Errors;

/// <summary>
///     Catalog of domain errors for the Trip bounded context, expressed in the ubiquitous language.
/// </summary>
public static class TripErrors
{
    /// <summary>The specified trip could not be found.</summary>
    public static readonly Error TripNotFound =
        new("Trip.TripNotFound", "The specified trip was not found.");

    /// <summary>A trip lifecycle transition was attempted from an invalid state.</summary>
    public static readonly Error InvalidTripState =
        new("Trip.InvalidTripState", "The trip is not in a valid state for this operation.");

    /// <summary>The trip could not be created with the provided data.</summary>
    public static readonly Error TripCreationFailed =
        new("Trip.TripCreationFailed", "An error occurred while creating the trip.");
}
