using BuildingBlocks.Abstractions;
using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Domain.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace EventSourcingDistilled.Api
{
    public static class Dependencies
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "",
                    Description = "",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "",
                        Email = ""
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });

                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(isOriginAllowed: _ => true)
                .AllowCredentials()));

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(GetCustomers));
            
            services.AddTransient<IEventStore, EventStore>();
            services.AddSingleton<IDateTime, MachineDateTime>();
            services.AddTransient<ICorrelationIdAccessor, CorrelationIdAccessor>();

            services.AddDbContext<EventSourcingDistilledDbContext>((options =>
            {
                options
                .LogTo(Console.WriteLine)
                .UseSqlServer(configuration["Data:DefaultConnection:ConnectionString"],
                    builder => builder
                    .MigrationsAssembly("EventSourcingDistilled.Api")
                        .EnableRetryOnFailure())
                .EnableSensitiveDataLogging();
            }));

            services.AddTransient<IEventSourcingDistilledDbContext, EventSourcingDistilledDbContext>();

            services.AddControllers();
        }
    }
}
