namespace Powertech.Platform.Fleet.Domain.Model.Commands;

/// <summary>
///     Command to define the weekdays on which a route operates.
/// </summary>
/// <remarks>Maps to the "Define Service Days" event-storming command, producing a <c>ServiceDaysDefined</c> event.</remarks>
/// <param name="RouteId">The route to configure.</param>
/// <param name="Days">The weekday names (e.g. MONDAY, WEDNESDAY).</param>
public record DefineServiceDaysCommand(Guid RouteId, IEnumerable<string> Days);