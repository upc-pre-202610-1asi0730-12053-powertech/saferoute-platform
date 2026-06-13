namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.model.commands;

/**
 * Command expressing the intent to complete a trip (US-14: "Finalización de Ruta").
 *
 * @param tripId identifier of the trip to complete
 */

public record CompleteTripCommand(long? tripId)
{
    
};