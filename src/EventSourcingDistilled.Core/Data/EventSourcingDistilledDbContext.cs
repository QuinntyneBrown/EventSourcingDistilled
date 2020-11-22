using BuildingBlocks.Domain;
using BuildingBlocks.EventStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Core.Data
{

    public class EventSourcingDistilledDbContext: IEventSourcingDistilledDbContext
    {
        private readonly IEventStore _eventStore;
        private readonly IRepository _repository;
        public EventSourcingDistilledDbContext(IEventStore eventStore, IRepository repository)
        {
            _eventStore = eventStore;
            _repository = repository;
        }
        public IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot: AggregateRoot
        {
            return _repository.Query<TAggregateRoot>().AsQueryable();
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            _eventStore.Save(aggregateRoot);
        }

        public TAggregateRoot Find<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot
        {
            return _repository.Query<TAggregateRoot>(id);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return 1;
        }
    }
}
