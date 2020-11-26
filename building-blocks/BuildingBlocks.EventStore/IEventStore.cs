using BuildingBlocks.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStore
    {
        void Store(AggregateRoot aggregateRoot);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
