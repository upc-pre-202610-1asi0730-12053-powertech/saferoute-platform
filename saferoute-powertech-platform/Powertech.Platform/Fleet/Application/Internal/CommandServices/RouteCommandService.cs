using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Fleet.Domain.Model;
using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Fleet.Domain.Model.Entities;
using Powertech.Platform.Fleet.Domain.Model.ValueObjects;
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
    public async Task<Result<Route>> Handle(CreateRouteCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var route = new Route(command);
            await routeRepository.AddAsync(route, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Route>.Success(route);
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

    /// <inheritdoc />
    public Task<Result<Route>> Handle(AddStopCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId,
            route => route.AddStop(command.Name, new Coordinates(command.Latitude, command.Longitude)),
            cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(RemoveStopCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.RemoveStop(new StopId(command.StopId)), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(AssignVehicleCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId,
            route => route.AssignVehicle(new Vehicle(route.OrganizationId, command.Plate, command.Model,
                command.Brand, command.Capacity)),
            cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(AssignDriverCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.AssignDriver(new DriverId(command.DriverId)), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(AssignStudentsToRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.AssignChild(new ChildId(command.ChildId)), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(RemoveStudentsFromRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.RemoveChild(new ChildId(command.ChildId)), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(DefineServiceDaysCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.DefineServiceDays(new ServiceDays(command.Days)),
            cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(SetDepartureTimeCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.SetDepartureTime(new DepartureTime(command.DepartureTime)),
            cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(ActivateRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.Activate(), cancellationToken);

    /// <inheritdoc />
    public Task<Result<Route>> Handle(DeactivateRouteCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.RouteId, route => route.Deactivate(), cancellationToken);

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
