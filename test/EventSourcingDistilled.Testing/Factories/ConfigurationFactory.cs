using EventSourcingDistilled.Api;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EventSourcingDistilled.Testing.Factories
{
    public static class ConfigurationFactory
    {
        private static IConfiguration configuration;
        public static IConfiguration Create()
        {
            if (configuration == null)
            {
                var basePath = Path.GetFullPath("../../../../../src/EventSourcingDistilled.Api");

                configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", false)
                    .Build();
            }

            return configuration;
        }
    }
}
