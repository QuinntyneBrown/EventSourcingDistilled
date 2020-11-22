using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EventStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services)
        {
            services.AddTransient<IEventStoreDbContext, EventStoreDbContext>();
            services.AddTransient<IAggregateSet, AggregateSet>();
            services.AddSingleton<IEventStore, EventStore>();
        }
    }
}
