using Powertech.Platform.Fleet.Domain.Model.Commands;
using Powertech.Platform.Fleet.Interfaces.Rest.Resources;

namespace Powertech.Platform.Fleet.Interfaces.Rest.Transform;

/// <summary>
///     Assembler that converts a <see cref="CreateRouteResource" /> into a <see cref="CreateRouteCommand" />.
/// </summary>
public static class CreateRouteCommandFromResourceAssembler
{
    /// <summary>Converts the input resource into a domain command, parsing the organization Guid.</summary>
    /// <param name="resource">The create-route resource. Must not be null.</param>
    /// <returns>The resulting <see cref="CreateRouteCommand" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource" /> is null.</exception>
    public static CreateRouteCommand ToCommandFromResource(CreateRouteResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateRouteCommand(Guid.Parse(resource.OrganizationId), resource.Name);
    }
}
