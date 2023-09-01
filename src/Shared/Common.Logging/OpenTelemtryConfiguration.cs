using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Logging
{
    public static class OpenTelemtryConfiguration
    {
        public static void AddAndConfigureOpenTelemetryTracing(this IHostApplicationBuilder host, Action<TracerProviderBuilder>? additionalTraces = null)
        {
            additionalTraces ??= (trace) => { };
            var zipkinUri = new Uri(host.Configuration.GetConnectionString("Zipkin") ?? throw new Exception("Zipkin ConnectionString not configured"));
            host.Services.AddOpenTelemetry()
               .ConfigureResource(resource => resource
                   .AddService(serviceName: host.Environment.ApplicationName))
               .WithTracing(sinks => sinks.DefaultTracing(zipkinUri))
               .WithTracing(additionalTraces);
        }
        private static TracerProviderBuilder DefaultTracing(this TracerProviderBuilder builder, Uri zipkinUri)
        {
            builder
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (httpContext) =>
                    {
                        // Filter out health check and elastic search serilog sink
                        return httpContext.Request.Path != "/hc";
                    };
                })
                .AddHttpClientInstrumentation()
                .AddZipkinExporter(options => options.Endpoint = zipkinUri);
            return builder;
        }
    }
}
