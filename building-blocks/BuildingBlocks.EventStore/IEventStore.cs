using BuildingBlocks.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStore
    {
        void Save(AggregateRoot aggregateRoot);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}
