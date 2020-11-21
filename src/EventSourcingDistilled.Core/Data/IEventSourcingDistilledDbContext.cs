﻿using BuildingBlocks.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Core.Data
{
    public interface IEventSourcingDistilledDbContext
    {
        IQueryable<T> Set<T>();
        void Add(AggregateRoot aggregateRoot);
        TAggregateRoot Find<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    }
}