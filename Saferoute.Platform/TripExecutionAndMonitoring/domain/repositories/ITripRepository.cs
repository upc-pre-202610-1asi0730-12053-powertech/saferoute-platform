using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;

namespace Saferoute.Platform.TripExecutionAndMonitoring.domain.repositories;

/**
 * Trip repository port.
 *
 * <p>Domain-facing abstraction for persisting and retrieving {@link Trip} aggregates together with
 * their attendance and incident children. The JPA-backed implementation lives in the infrastructure
 * layer, keeping the domain persistence-agnostic.</p>
 */
public interface ITripRepository {

    /**
     * Persists the given trip (insert or update) and returns the saved aggregate.
     *
     * @param trip the trip to save
     * @return the persisted trip, carrying its assigned identity
     */
    Trip save(Trip trip);

    /**
     * Finds a trip by its identity.
     *
     * @param id the trip identity
     * @return the trip if present, otherwise empty
     */
    Optional<Trip> findById(long? id);
}