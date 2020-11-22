using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStoreDbContext
    {
        DbSet<StoredEvent> StoredEvents { get; }
        DbSet<SnapShot> SnapShots { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
