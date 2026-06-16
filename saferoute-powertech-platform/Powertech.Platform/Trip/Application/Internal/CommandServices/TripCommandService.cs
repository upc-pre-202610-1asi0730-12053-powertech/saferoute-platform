using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Trip.Application.CommandServices;
using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Domain.Model;
using Powertech.Platform.Trip.Domain.Repositories;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;


namespace Powertech.Platform.Trip.Application.Internal;

/// <summary>
///     Default implementation of <see cref="ITripCommandService" />.
/// </summary>
/// <remarks>
///     Orchestrates the Trip aggregate behavior and persistence. Domain invariant violations
///     surfaced by the aggregate (as <see cref="InvalidOperationException" />) and value-object
///     validation (as <see cref="ArgumentException" />) are translated into typed
///     <see cref="Result{T}" /> failures, keeping exceptions out of the normal control flow exposed
///     to the interface layer.
/// </remarks>
/// <param name="tripRepository">The trip repository abstraction.</param>
/// <param name="unitOfWork">The unit of work used to commit changes atomically.</param>
/// <param name="localizer">The localizer used to resolve error messages.</param>
public class TripCommandService(
    ITripRepository tripRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : ITripCommandService
{
    /// <inheritdoc />
    public async Task<Result<TripAggregate>> Handle(CreateTripCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var trip = new TripAggregate(command);
            await tripRepository.AddAsync(trip, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<TripAggregate>.Success(trip);
        }
        catch (ArgumentException ex)
        {
            return Result<TripAggregate>.Failure(TripError.InvalidTripData, ex.Message);
        }
        catch (OperationCanceledException)
        {
            return Result<TripAggregate>.Failure(TripError.OperationCancelled,
                localizer[nameof(TripError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<TripAggregate>.Failure(TripError.DatabaseError, localizer[nameof(TripError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<TripAggregate>.Failure(TripError.InternalServerError,
                localizer[nameof(TripError.InternalServerError)]);
        }
    }
    
    /// <inheritdoc />
    public Task<Result<TripAggregate>> Handle(StartTripCommand command, CancellationToken cancellationToken) =>
        MutateAsync(command.TripId, trip => trip.Start(), cancellationToken);
    
        /// <summary>
    ///     Shared workflow for commands that load an existing trip, mutate it through a domain
    ///     behavior and persist the change, mapping any failure to a typed result.
    /// </summary>
    /// <param name="tripId">The identifier of the trip to load.</param>
    /// <param name="mutation">The domain behavior to apply to the loaded aggregate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result wrapping the mutated trip, or a typed failure.</returns>
    private async Task<Result<TripAggregate>> MutateAsync(
        Guid tripId,
        Action<TripAggregate> mutation,
        CancellationToken cancellationToken)
    {
        try
        {
            var trip = await tripRepository.FindByTripIdAsync(new TripId(tripId), cancellationToken);
            if (trip is null)
                return Result<TripAggregate>.Failure(TripError.TripNotFound, localizer[nameof(TripError.TripNotFound)]);

            mutation(trip);

            tripRepository.Update(trip);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<TripAggregate>.Success(trip);
        }
        catch (InvalidOperationException ex)
        {
            return Result<TripAggregate>.Failure(TripError.InvalidTripState, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result<TripAggregate>.Failure(TripError.InvalidTripData, ex.Message);
        }
        catch (OperationCanceledException)
        {
            return Result<TripAggregate>.Failure(TripError.OperationCancelled,
                localizer[nameof(TripError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<TripAggregate>.Failure(TripError.DatabaseError, localizer[nameof(TripError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<TripAggregate>.Failure(TripError.InternalServerError,
                localizer[nameof(TripError.InternalServerError)]);
        }
    }
    
    
}