using Powertech.Platform.Trip.Domain.Model.Commands;
using Powertech.Platform.Trip.Interfaces.Rest.Resources;

namespace Powertech.Platform.Trip.Interfaces.Rest.Transform;

/// <summary>
///     Assembler that converts a <see cref="SetBoardingStatusResource" /> and a trip identifier into a
///     <see cref="SetBoardingStatusCommand" />.
/// </summary>

public class SetBoardingStatusCommandFromResourceAssembler
{
    /// <summary>Builds the command from the route parameter and request body.</summary>
    /// <param name="tripId">The trip identifier taken from the route.</param>
    /// <param name="resource">The boarding resource from the request body. Must not be null.</param>
    /// <returns>The resulting <see cref="SetBoardingStatusCommand" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource" /> is null.</exception>
    public static SetBoardingStatusCommand ToCommandFromResource(Guid tripId, SetBoardingStatusResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SetBoardingStatusCommand(
            tripId,
            Guid.Parse(resource.ChildId),
            resource.BoardingState);
    } 
}