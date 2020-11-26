using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BuildingBlocks.EventStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services, EventStoreBuilderOptions eventStoreBuilderOptions)
        {
            services.AddTransient<IEventStoreDbContext, EventStoreDbContext>();
            services.AddTransient<IAggregateSet, AggregateSet>();
            services.AddTransient<IEventStore, EventStore>();
            services.AddSingleton<IDateTime, MachineDateTime>();
            services.AddDbContext<EventStoreDbContext>(options =>
            {
                options.UseSqlServer(eventStoreBuilderOptions.ConnectionString,
                    builder => builder.MigrationsAssembly(eventStoreBuilderOptions.MigrationAssembly)
                        .EnableRetryOnFailure())
                .UseLoggerFactory(EventStoreDbContext.ConsoleLoggerFactory)
                .EnableSensitiveDataLogging();
            });
        }

    }

    public class MachineDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
    public class EventStoreBuilderOptions
    {
        public string MigrationAssembly { get; set; }
        public string ConnectionString { get; set; }
    }
}
