using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Shopping.Aggregator.Services;


// The policies might feel like duplicates but each service might need different fine tuned policy so it's better to keep them separate.
var retryPolicy = (IServiceProvider services,HttpRequestMessage request) => HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(4, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), (outcome, timespan, retryAttempt, context) =>
            {
                Log.Error("Request failed HTTP/{Version} {Method} {Url} - Delaying for {Delay}s, then making retry {Retry}. {Outcome}", request.Version?.ToString(), request.Method?.Method, request.RequestUri?.ToString(), timespan.TotalSeconds, retryAttempt, outcome.Exception?.Message);
            });

var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30), (outcome,timeSpan) => 
            { 
                Log.Error("Circuit breaker opened for the next {TimeSpan}s due to {Outcome}", timeSpan.TotalSeconds, outcome.Exception?.Message);
            }, () => 
            { 
                Log.Information("Circuit breaker reset.");
            });

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:CatalogUrl"] + "/swagger/index.html" ?? throw new Exception("CatalogUrl configuration not found")), name: "catalog-api", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:BasketUrl"] + "/swagger/index.html" ?? throw new Exception("BasketUrl configuration not found")), name: "basket-api", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:OrderingUrl"] + "/swagger/index.html" ?? throw new Exception("OrderingUrl configuration not found")), name: "ordering-api", HealthStatus.Degraded);
builder.AddAndConfigureOpenTelemetryTracing();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"] ?? throw new Exception("CatalogUrl configuration not found")))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"] ?? throw new Exception("BasketUrl configuration not found")))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"] ?? throw new Exception("OrderingUrl configuration not found")))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
