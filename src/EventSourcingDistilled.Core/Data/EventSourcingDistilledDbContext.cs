// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcingDistilled.Core.CustomerAggregateModel;
using Microsoft.EntityFrameworkCore;
using System;


namespace EventSourcingDistilled.Core.Data;

public class EventSourcingDistilledDbContext : EventStore, IEventSourcingDistilledDbContext
{
    public EventSourcingDistilledDbContext(DbContextOptions options, IDateTime dateTime, ICorrelationIdAccessor correlationIdAccessor)
        : base(options, dateTime, correlationIdAccessor)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Customer> Customers { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(e => !e.Deleted.HasValue);
    }

    protected override void OnTrackedAggregatesChanged(IAggregateRoot aggregateRoot, EntityState entityState)
    {
        if (Entry(aggregateRoot).State == EntityState.Detached)
        {
            Attach(aggregateRoot);

            Entry(aggregateRoot).State = entityState;

        }
    }
}

