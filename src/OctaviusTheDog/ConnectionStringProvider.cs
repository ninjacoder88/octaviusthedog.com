using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace OctaviusTheDog
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string name);
    }

    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ConnectionStringProvider(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string GetConnectionString(string name)
        {
            if (webHostEnvironment.IsProduction())
            {
                return Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{name}");
            }

            return configuration.GetConnectionString(name);
        }
    }
}
