using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public static class SerilogConfiguration
    {
        public static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration configuration)
        {
            var elasticUri = new Uri(context.Configuration.GetConnectionString("ElasticSearch") ?? throw new Exception("ElasticSearch ConnectionString not configured"));

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new(elasticUri)
                {
                    IndexFormat = $"applogs-{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1
                })
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName!)
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName!)
                .ReadFrom.Configuration(context.Configuration);
        }
    }
}
