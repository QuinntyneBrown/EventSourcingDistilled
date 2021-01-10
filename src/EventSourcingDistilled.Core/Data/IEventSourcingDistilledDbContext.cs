using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingDistilled.Core.Data
{
    public interface IEventSourcingDistilledDbContext : IEventStore
    {
        DbSet<Customer> Customers { get; }
    }
}
