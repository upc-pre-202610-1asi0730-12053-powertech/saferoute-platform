using Cortex.Mediator;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.model.aggregates;
using Saferoute.Platform.TripExecutionAndMonitoring.domain.repositories;

namespace Saferoute.Platform.TripExecutionAndMonitoring.infrastructure.persistence.adapters;

/// <summary>
/// Repository adapter bridging the <see cref="ITripRepository"/> domain port with Entity Framework.
/// <para>
/// Acts as the domain-event publishing boundary for the Trip context: events registered on the
/// aggregate during a mutation (boarding update, incident report, completion) are published after a
/// successful save, and the <c>TripStartedEvent</c> is registered and published once the identity
/// is available for a newly started trip.
/// </para>
/// </summary>
public class TripRepository : ITripRepository
{
    private readonly ITripPersistenceRepository _tripPersistenceRepository;
    private readonly IMediator _eventPublisher;

    /// <summary>
    /// Creates the adapter with its database repository and the event publisher.
    /// </summary>
    /// <param name="tripPersistenceRepository">The persistence repository (DbContext/EF)</param>
    /// <param name="eventPublisher">The application event publisher</param>
    public TripRepository(ITripPersistenceRepository tripPersistenceRepository, IMediator eventPublisher)
    {
        _tripPersistenceRepository = tripPersistenceRepository;
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public Trip Save(Trip trip)
    {
        bool isNew = trip.Id is null; // Equivalente a Long == null

        // Capture events registered during this unit of work (boarding, incident, completion).
        var pendingEvents = new List<object>(trip.DomainEvents());
        trip.ClearDomainEvents();

        var savedEntity = _tripPersistenceRepository.Save(TripPersistenceAssembler.ToPersistenceFromDomain(trip));
        var savedTrip = TripPersistenceAssembler.ToDomainFromPersistence(savedEntity);

        if (isNew)
        {
            savedTrip.OnStarted();
            pendingEvents.AddRange(savedTrip.DomainEvents());
            savedTrip.ClearDomainEvents();
        }
        
        foreach (var domainEvent in pendingEvents)
        {
            _eventPublisher.Publish(domainEvent);
        }

        return savedTrip;
    }
    
    public Trip? FindById(long id)
    {
        var entity = _tripPersistenceRepository.FindById(id);
        
        return entity is null ? null : TripPersistenceAssembler.ToDomainFromPersistence(entity);
    }
}