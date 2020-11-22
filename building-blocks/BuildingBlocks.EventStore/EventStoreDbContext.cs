using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EventStore
{
    public class EventStoreDbContext: DbContext, IEventStoreDbContext
    {
        public EventStoreDbContext(DbContextOptions options)
            : base(options) { }
        public DbSet<StoredEvent> StoredEvents { get; private set; }
        public DbSet<SnapShot> SnapShots { get; private set; }
    }
}
