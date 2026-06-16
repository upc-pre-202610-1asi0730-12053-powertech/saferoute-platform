using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Trip.Application.QueryServices;
using Powertech.Platform.Trip.Domain.Model.Queries;
using Powertech.Platform.Trip.Domain.Repositories;
using TripAggregate = Powertech.Platform.Trip.Domain.Model.Aggregates.Trip;

namespace Powertech.Platform.Trip.Application.Internal.QueryServices;

/// <summary>
///     Default implementation of <see cref="ITripQueryService" />.
/// </summary>
/// <remarks>Depends on the repository abstraction only, never on its implementation.</remarks>
/// <param name="tripRepository">The trip repository.</param>
public class TripQueryService(ITripRepository tripRepository) : ITripQueryService
{
    /// <inheritdoc />
    public async Task<TripAggregate?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken)
    {
        return await tripRepository.FindByTripIdAsync(new TripId(query.TripId), cancellationToken);
    }
}