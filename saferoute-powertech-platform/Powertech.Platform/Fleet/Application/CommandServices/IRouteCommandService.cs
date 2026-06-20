using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Shared.Application.Model;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Application.CommandServices;

/// <summary>
///     Application service that handles the write operations (commands) of the Fleet context.
/// </summary>
/// <remarks>
///     Every handler returns a <see cref="Result{T}" /> wrapping the affected <see cref="Route" />
///     aggregate, so the interface layer can translate success/failure without exceptions.
/// </remarks>
public interface IRouteCommandService
{
    /// <summary>Handles assigning a child to a route.</summary>
    Task<Result<Route>> Handle(AssignStudentsToRouteCommand command, CancellationToken cancellationToken);
    
    /// <summary>Handles removing a child from a route.</summary>
    Task<Result<Route>> Handle(RemoveStudentsFromRouteCommand command, CancellationToken cancellationToken);
    
}
