// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace EventSourcing;

public interface IEventStore
{
    DbSet<StoredEvent> StoredEvents { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
        where TAggregateRoot : AggregateRoot;
    void Add(IAggregateRoot aggregateRoot);
    ChangeTracker ChangeTracker { get; }
}

