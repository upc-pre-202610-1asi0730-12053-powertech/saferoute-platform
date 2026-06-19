using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Interfaces.Rest.Resources;

namespace Powertech.Platform.Trip.Interfaces.Rest.Transform;

/// <summary>
///     Assembler that converts a <see cref="ReportIncidentResource" /> and a trip identifier into a
///     <see cref="ReportIncidentCommand" />.
/// </summary>
public static class ReportIncidentCommandFromResourceAssembler
{
    /// <summary>Builds the command from the route parameter and request body.</summary>
    /// <param name="tripId">The trip identifier taken from the route.</param>
    /// <param name="resource">The incident resource from the request body. Must not be null.</param>
    /// <returns>The resulting <see cref="ReportIncidentCommand" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource" /> is null.</exception>
    public static ReportIncidentCommand ToCommandFromResource(Guid tripId, ReportIncidentResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ReportIncidentCommand(tripId, resource.Description);
    }
}