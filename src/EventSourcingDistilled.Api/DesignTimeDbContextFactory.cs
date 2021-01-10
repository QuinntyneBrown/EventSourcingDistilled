/*using BuildingBlocks.Abstractions;
using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Api
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EventSourcingDistilledDbContext>
    {
        public EventSourcingDistilledDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            return new EventSourcingDistilledDbContext(new DbContextOptionsBuilder()
                .UseSqlServer(configuration["Data:DefaultConnection:ConnectionString"])
                .Options, null, null);
        }
    }
}
*/