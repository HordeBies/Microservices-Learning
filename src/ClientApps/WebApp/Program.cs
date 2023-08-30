using Common.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using WebApp.Services;

// The policies might feel like duplicates but each service might need different fine tuned policy/logging so it's better to keep them separate.
var retryPolicy = (IServiceProvider services, HttpRequestMessage request) => HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(4, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), (outcome, timespan, retryAttempt, context) =>
            {
                Log.Error("Request failed HTTP/{Version} {Method} {Url} - Delaying for {Delay}s, then making retry {Retry}. {Outcome}", request.Version?.ToString(), request.Method?.Method, request.RequestUri?.ToString(), timespan.TotalSeconds, retryAttempt, outcome.Exception?.Message);
            });

var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30), (outcome, timeSpan) =>
            {
                Log.Error("Circuit breaker opened for the next {TimeSpan}s due to {Outcome}", timeSpan.TotalSeconds, outcome.Exception?.Message);
            }, () =>
            {
                Log.Information("Circuit breaker reset.");
            });

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);

// Add services to the container.
builder.Services.AddRazorPages();

var baseAddress = builder.Configuration["ApiSettings:GatewayAddress"] ?? throw new Exception("GatewayAddress not configured!");
builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
