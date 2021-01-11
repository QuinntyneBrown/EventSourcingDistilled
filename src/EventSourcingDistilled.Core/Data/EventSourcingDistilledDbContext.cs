using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventSourcingDistilled.Core.Data
{
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
            try
            {
                if (Entry(aggregateRoot).State == EntityState.Detached)
                {
                    Attach(aggregateRoot);

                    Entry(aggregateRoot).State = entityState;

                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
