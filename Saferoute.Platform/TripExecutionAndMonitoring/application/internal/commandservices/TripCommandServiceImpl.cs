using MySqlX.XDevAPI.Common;
using Saferoute.Platform.TripExecutionAndMonitoring.application.commandservices;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.commands;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.repositories;

namespace Saferoute.Platform.TripExecutionAndMonitoring.Application.Internal.Commandservices;

public class TripCommandServiceImpl : ITripCommandService
{
    private readonly ITripRepository tripRepository;
    
    /**
     * Creates the service with its required repository port.
     *
     * @param tripRepository the trip repository port
     */
    public TripCommandServiceImpl(ITripRepository tripRepository) {
        this.tripRepository = tripRepository;
    }
    
    /// <inheritdoc />
    public Result<Trip, ApplicationError> Handle(CompleteTripCommand command)
    {
        try
        {
            var trip = tripRepository.FindById(command.TripId);
        
            if (trip is null)
            {
                return Result.Failure(ApplicationError.NotFound("Trip", command.TripId.ToString()));
            }

            trip.Complete();
            return Result.Success(tripRepository.Save(trip));
        }
        catch (InvalidOperationException e)
        {
            return Result.Failure(ApplicationError.BusinessRuleViolation("Trip completion", e.Message));
        }
        catch (Exception e)
        {
            return Result.Failure(ApplicationError.Unexpected("Trip completion", e.Message));
        }
    }
}