using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Fleet.Application.CommandServices;
using Powertech.Platform.Fleet.Domain.Model;
using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Fleet.Domain.Repositories;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Application.Internal.CommandServices;

/// <summary>
///     Default implementation of <see cref="IRouteCommandService" />.
/// </summary>
/// <remarks>
///     Orchestrates the Route aggregate behavior and persistence. Domain invariant violations
///     (<see cref="InvalidOperationException" />) and value-object validation
///     (<see cref="ArgumentException" />) are translated into typed <see cref="Result{T}" /> failures.
/// </remarks>
/// <param name="routeRepository">The route repository abstraction.</param>
/// <param name="unitOfWork">The unit of work used to commit changes atomically.</param>
/// <param name="localizer">The localizer used to resolve error messages.</param>
public class RouteCommandService(
    IRouteRepository routeRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IRouteCommandService
{
    /// <inheritdoc />
    public Task<Result<Route>> Handle(AssignStudentsToRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.AssignStudent(new ChildId(command.ChildId)), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(RemoveStudentsFromRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.RemoveStudent(new ChildId(command.ChildId)), cancellationToken);

    /// <inheritdoc />
    /// <summary>
    ///     Shared workflow for commands that load an existing route, mutate it through a domain
    ///     behavior and persist the change, mapping any failure to a typed result.
    /// </summary>
    /// <param name="routeId">The identifier of the route to load.</param>
    /// <param name="mutation">The domain behavior to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task<Result<Route>> MutateAsync(
        Guid routeId,
        Action<Route> mutation,
        CancellationToken cancellationToken)
    {
        try
        {
            var route = await routeRepository.FindByRouteIdAsync(new RouteId(routeId), cancellationToken);
            if (route is null)
                return Result<Route>.Failure(FleetError.RouteNotFound, localizer[nameof(FleetError.RouteNotFound)]);

            mutation(route);

            routeRepository.Update(route);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Route>.Success(route);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Route>.Failure(FleetError.InvalidRouteState, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result<Route>.Failure(FleetError.InvalidRouteData, ex.Message);
        }
        catch (OperationCanceledException)
        {
            return Result<Route>.Failure(FleetError.OperationCancelled,
                localizer[nameof(FleetError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Route>.Failure(FleetError.DatabaseError, localizer[nameof(FleetError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Route>.Failure(FleetError.InternalServerError,
                localizer[nameof(FleetError.InternalServerError)]);
        }
    }
}