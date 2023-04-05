// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static EventSourcing.AggregateRoot;
using static Newtonsoft.Json.JsonConvert;


namespace EventSourcing;

public class EventStore : DbContext, IEventStore
{
    private readonly List<StoredEvent> _changes = new List<StoredEvent>();
    private readonly IDateTime _dateTime;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    protected virtual void OnTrackedAggregatesChanged(IAggregateRoot aggregateRoot,EntityState entityState)
    {

    }

    protected readonly List<IAggregateRoot> _trackedAggregates = new List<IAggregateRoot>();
    protected List<IAggregateRoot> TrackedAggregates { get; }
    public EventStore(DbContextOptions options, IDateTime dateTime, ICorrelationIdAccessor correlationIdAccessor)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        _dateTime = dateTime;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public DbSet<StoredEvent> StoredEvents { get; protected set; }

    public async Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
        where TAggregateRoot : AggregateRoot
    {
        var events = (await StoredEvents.Where(x => x.StreamId == id).OrderBy(x => x.CreatedOn).ToListAsync())
                .Select(x => DeserializeObject(x.Data, Type.GetType(x.DotNetType)) as IEvent);

        if (!events.Any())
            return null;

        var aggregate = events.Aggregate(Create<TAggregateRoot>(), (x, y) => x.Apply(y) as TAggregateRoot);

        aggregate.ClearChanges();

        _trackedAggregates.Add(aggregate);

        OnTrackedAggregatesChanged(aggregate, EntityState.Modified);

        return aggregate;
    }

    public void Add(IAggregateRoot aggregateRoot)
    {
        _trackedAggregates.Add(aggregateRoot);

        OnTrackedAggregatesChanged(aggregateRoot, EntityState.Added);

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        foreach (var aggregateRoot in _trackedAggregates)
        {
            var type = aggregateRoot.GetType();

            StoredEvents.AddRange(aggregateRoot.DomainEvents
                .Select(@event => {
                    var type = aggregateRoot.GetType();

                    return new StoredEvent
                    {
                        StoredEventId = Guid.NewGuid(),
                        Aggregate = aggregateRoot.GetType().Name,
                        AggregateDotNetType = aggregateRoot.GetType().AssemblyQualifiedName,
                        Data = SerializeObject(@event),
                        StreamId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregateRoot, null),
                        DotNetType = @event.GetType().AssemblyQualifiedName,
                        Type = @event.GetType().Name,
                        CreatedOn = @event.Created,
                        CorrelationId = _correlationIdAccessor.CorrelationId
                    };
                }));
        }

        _trackedAggregates.Clear();

        return await base.SaveChangesAsync(cancellationToken);
    }
}


