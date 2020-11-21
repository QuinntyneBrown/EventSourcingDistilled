using BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Core.Data
{

    public class EventSourcingDistilledDbContext: IEventSourcingDistilledDbContext
    {
        public IQueryable<T> Set<T>()
        {
            throw new NotImplementedException();
        }

        public void Add(AggregateRoot aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot Find<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
