namespace Powertech.Platform.Fleet.Interfaces.Rest.Resources;
    
    
/// <summary>Input resource to define the service days of a route.</summary>
/// <param name="Days">The weekday names (e.g. MONDAY, WEDNESDAY).</param>
public record DefineServiceDaysResource(IReadOnlyList<string> Days);
