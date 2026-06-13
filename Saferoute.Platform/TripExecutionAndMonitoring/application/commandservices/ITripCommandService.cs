using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.commands;

namespace Saferoute.Platform.TripExecutionAndMonitoring.application.commandservices;

/**
 * Application service contract for Trip write operations within the Trip bounded context.
 *
 * <p>Each handler returns a {@link Result} carrying either the affected {@link Trip} aggregate or an
 * {@link ApplicationError}, making validation and business-rule failures explicit.</p>
 */

public record ITripCommandService
{
    
    /**
     * Handles completing a trip (US-14).
     *
     * @param command the {@link CompleteTripCommand}
     * @return the completed trip on success, or an error if it does not exist or cannot be completed
     */
    Result<Trip, ApplicationError> Handle(CompleteTripCommand command);    
    
};