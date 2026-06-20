using Powertech.Platform.Fleet.Domain.Model.Entities;
using Powertech.Platform.Fleet.Interfaces.Rest.Resources;
using Route = Powertech.Platform.Fleet.Domain.Model.Aggregates.Route;

namespace Powertech.Platform.Fleet.Interfaces.Rest.Transform;

/// <summary>
///     Assembler that converts a <see cref="Route" /> aggregate into its <see cref="RouteResource" />
///     representation, including its stops, vehicle and assignment.
/// </summary>
public static class RouteResourceFromEntityAssembler
{
    /// <summary>Converts the aggregate into the published route resource.</summary>
    /// <param name="entity">The route aggregate. Must not be null.</param>
    /// <returns>The resulting <see cref="RouteResource" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity" /> is null.</exception>
    public static RouteResource ToResourceFromEntity(Route entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new RouteResource(
            entity.Id.ToString(),
            entity.OrganizationId.ToString(),
            entity.Name,
            entity.State.Value,
            entity.Assignment is null ? null : ToAssignmentResource(entity.Assignment));
    }

    /// <summary>Converts an <see cref="Assignment" /> entity into its resource.</summary>
    private static AssignmentResource ToAssignmentResource(Assignment assignment) =>
        new(assignment.Id.ToString(),
            assignment.DriverId.ToString(),
            assignment.Children.Select(child => child.ToString()).ToList());
}
