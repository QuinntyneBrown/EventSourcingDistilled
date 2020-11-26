using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BuildingBlocks.EventStore
{
    public class EventStoreDbContext: DbContext, IEventStoreDbContext
    {
        public EventStoreDbContext(DbContextOptions options)
            : base(options) { }

        public static readonly ILoggerFactory ConsoleLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public DbSet<StoredEvent> StoredEvents { get; private set; }
        public DbSet<SnapShot> SnapShots { get; private set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SnapShot>(x =>
            {
                x.Property(o => o.Data).HasConversion(
                    data => JsonConvert.SerializeObject(data),
                    data => JsonConvert.DeserializeObject<Dictionary<string, HashSet<AggregateRoot>>>(data));
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
