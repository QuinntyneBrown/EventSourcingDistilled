using Microsoft.EntityFrameworkCore;
using EventSourcingDistilled.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BuildingBlocks.Abstractions;
using System;

namespace EventSourcingDistilled.Core.Data
{
    public interface IEventSourcingDistilledDbContext
    {
        DbSet<Customer> Customers { get; }
        void Add(IAggregateRoot aggregate);
        Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
