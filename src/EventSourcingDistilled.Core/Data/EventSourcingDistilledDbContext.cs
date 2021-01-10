using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingDistilled.Core.Data
{
    public class EventSourcingDistilledDbContext : EventStore, IEventSourcingDistilledDbContext
    {
        public EventSourcingDistilledDbContext(DbContextOptions options, IDateTime dateTime, ICorrelationIdAccessor correlationIdAccessor)
            : base(options, dateTime, correlationIdAccessor)
        {
        }

        public DbSet<Customer> Customers { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasQueryFilter(e => !e.Deleted.HasValue);
        }
    }
}
