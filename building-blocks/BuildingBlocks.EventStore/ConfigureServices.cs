// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static void AddEventSourcingServices(this IServiceCollection services)
    {
        services.AddTransient<IEventStore, EventStore>();
        services.AddSingleton<IDateTime, MachineDateTime>();
        services.AddTransient<ICorrelationIdAccessor, CorrelationIdAccessor>();
    }
}

