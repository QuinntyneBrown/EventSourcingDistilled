using BuildingBlocks.Abstractions;
using BuildingBlocks.EventStore;
using EventSourcingDistilled.Api;
using EventSourcingDistilled.Testing.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using static EventSourcingDistilled.Core.Constants.ConfigurationKeys;

namespace EventSourcingDistilled.Testing
{
    public class ApiTestFixture : WebApplicationFactory<Startup>, IDisposable
    {
        private BuildingBlocks.Abstractions.IAppDbContext _context;
        private IConfiguration _configuration;
        private readonly Guid _correlationId = Guid.NewGuid();

        public ApiTestFixture()
        {
            _configuration = ConfigurationFactory.Create();
        }

        public new HttpClient CreateClient()
        {
            var client = base.CreateClient();

            client.DefaultRequestHeaders.Add("correlationId", $"{_correlationId}");

            return client;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {

                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var context = scopedServices.GetRequiredService<BuildingBlocks.Abstractions.IAppDbContext>();

                    

                }
            });
        }
        public BuildingBlocks.Abstractions.IAppDbContext Context
        {
            get
            {
                if (_context == null)
                {
                    var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                        .UseSqlServer(_configuration[DataDefaultConnectionString])
                        .Options;

                    var context = new EventStoreDbContext(options);
                    var dateTime = new MachineDateTime();
                    var eventStore = new EventStore(context, dateTime, new TestCorrelationIdAccessor(_correlationId));
                    var aggregateSet = new AggregateSet(context, dateTime);
                    _context = new AppDbContext(eventStore, aggregateSet);
                }

                return _context;
            }
            set
            {
                _context = value;
            }
        }




        
        protected override void Dispose(bool disposing)
        {
            var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                .UseSqlServer(_configuration[DataDefaultConnectionString])
                .Options;

            var context = new EventStoreDbContext(options);

            foreach (var storedEvent in context.StoredEvents.Where(x => x.CorrelationId == _correlationId))
            {
                context.Remove(storedEvent);
            }

            context.SaveChanges();

            base.Dispose(disposing);
        }
    }

    public class TestCorrelationIdAccessor : ICorrelationIdAccessor
    {
        private Guid _correlationId;
        public TestCorrelationIdAccessor(Guid correlationId)
        {
            _correlationId = correlationId;
        }
        public Guid CorrelationId => _correlationId;
    }
}
