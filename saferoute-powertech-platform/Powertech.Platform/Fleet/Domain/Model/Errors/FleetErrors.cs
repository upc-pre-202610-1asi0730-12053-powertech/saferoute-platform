using Powertech.Platform.Shared.Domain.Model;

namespace Powertech.Platform.Fleet.Domain.Model.Errors;

/// <summary>
///     Catalog of domain errors for the Fleet bounded context, expressed in the ubiquitous language.
/// </summary>
public static class FleetErrors
{
    /// <summary>The specified route could not be found.</summary>
    public static readonly Error RouteNotFound =
        new("Fleet.RouteNotFound", "The specified route was not found.");

    /// <summary>A route operation was attempted from an invalid state.</summary>
    public static readonly Error InvalidRouteState =
        new("Fleet.InvalidRouteState", "The route is not in a valid state for this operation.");

    /// <summary>The route could not be created with the provided data.</summary>
    public static readonly Error RouteCreationFailed =
        new("Fleet.RouteCreationFailed", "An error occurred while creating the route.");
}
