using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Trip.Domain.Model.Commands;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Application.CommandServices;

/// <summary>
///     Application service that handles the write operations (commands) of the Trip context.
/// </summary>
/// <remarks>
///     Each handler orchestrates the aggregate behavior and persistence, returning a
///     <see cref="Result{T}" /> so the interface layer can translate success/failure without
///     relying on exceptions for control flow.
/// </remarks>
public interface ITripCommandService
{
    /// <summary>Handles the preparation (creation) of a new trip.</summary>
    /// <param name="command">The create-trip command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result wrapping the created trip.</returns>
    Task<Result<TripAggregate>> Handle(CreateTripCommand command, CancellationToken cancellationToken);
    
    /// <summary>Handles starting a prepared trip.</summary>
    Task<Result<TripAggregate>> Handle(StartTripCommand command, CancellationToken cancellationToken);
    
    /// <summary>Handles recording a child's boarding status during a trip.</summary>
    Task<Result<TripAggregate>> Handle(SetBoardingStatusCommand command, CancellationToken cancellationToken);
    
    /// <summary>Handles reporting an incident during a trip.</summary>
    Task<Result<TripAggregate>> Handle(ReportIncidentCommand command, CancellationToken cancellationToken);
    
    /// <summary>Handles completing an in-progress trip.</summary>
    Task<Result<TripAggregate>> Handle(CompleteTripCommand command, CancellationToken cancellationToken);
}