using Route = Safer_Route_Platform.Fleet.Domain.Model.Aggregates.Route;
using Safer_Route_Platform.Fleet.Domain.Model.Commands;
using Safer_Route_Platform.Shared.Application.Model;

namespace Safer_Route_Platform.Fleet.Application.CommandServices;

/// <summary>
///     Application service that handles the write operations (commands) of the Fleet context.
/// </summary>
/// <remarks>
///     Every handler returns a <see cref="Result{T}" /> wrapping the affected <see cref="Route" />
///     aggregate, so the interface layer can translate success/failure without exceptions.
/// </remarks>
public interface IRouteCommandService
{
    /// <summary>Handles defining (creating) a new route.</summary>
    Task<Result<Route>> Handle(CreateRouteCommand command, CancellationToken cancellationToken);

    /// <summary>Handles appending a stop to a route.</summary>
    Task<Result<Route>> Handle(AddStopCommand command, CancellationToken cancellationToken);

    /// <summary>Handles removing a stop from a route.</summary>
    Task<Result<Route>> Handle(RemoveStopCommand command, CancellationToken cancellationToken);

    /// <summary>Handles assigning a vehicle to a route.</summary>
    Task<Result<Route>> Handle(AssignVehicleCommand command, CancellationToken cancellationToken);

    /// <summary>Handles assigning the operating driver to a route.</summary>
    Task<Result<Route>> Handle(AssignDriverCommand command, CancellationToken cancellationToken);

    /// <summary>Handles assigning a child to a route.</summary>
    Task<Result<Route>> Handle(AssignChildToRouteCommand command, CancellationToken cancellationToken);

    /// <summary>Handles removing a child from a route.</summary>
    Task<Result<Route>> Handle(RemoveChildFromRouteCommand command, CancellationToken cancellationToken);

    /// <summary>Handles defining the service days of a route.</summary>
    Task<Result<Route>> Handle(DefineServiceDaysCommand command, CancellationToken cancellationToken);

    /// <summary>Handles setting the departure time of a route.</summary>
    Task<Result<Route>> Handle(SetDepartureTimeCommand command, CancellationToken cancellationToken);

    /// <summary>Handles finalizing the setup and activating a route.</summary>
    Task<Result<Route>> Handle(ActivateRouteCommand command, CancellationToken cancellationToken);

    /// <summary>Handles deactivating an active route.</summary>
    Task<Result<Route>> Handle(DeactivateRouteCommand command, CancellationToken cancellationToken);
}
