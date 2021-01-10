using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BuildingBlocks.EventStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services)
        {
            services.AddTransient<IEventStore, EventStore>();
            services.AddSingleton<IDateTime, MachineDateTime>();
            services.AddTransient<ICorrelationIdAccessor, CorrelationIdAccessor>();
        }
    }
}
