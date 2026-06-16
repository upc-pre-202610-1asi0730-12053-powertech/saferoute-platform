using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Interfaces.Rest.Resources;

namespace Powertech.Platform.Trip.Interfaces.Rest.Transform;

/// <summary>
///     Assembler that converts a <see cref="CreateTripResource" /> into a <see cref="CreateTripCommand" />.
/// </summary>
public static class CreateTripCommandFromResourceAssembler
{
    /// <summary>Converts the input resource into a domain command, parsing the Guid identifiers.</summary>
    /// <param name="resource">The create-trip resource. Must not be null.</param>
    /// <returns>The resulting <see cref="CreateTripCommand" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource" /> is null.</exception>
    /// <exception cref="FormatException">Thrown when an identifier is not a valid Guid.</exception>
    public static CreateTripCommand ToCommandFromResource(CreateTripResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateTripCommand(
            Guid.Parse(resource.OrganizationId),
            Guid.Parse(resource.RouteId),
            Guid.Parse(resource.DriverId));
    }
}