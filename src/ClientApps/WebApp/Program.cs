using Common.Logging;
using HealthChecks.UI.Client;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net;
using System.Security.Authentication;
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
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:GatewayAddress"] ?? throw new Exception("GatewayAddress configuration not found")), name: "ocelot-api-gateway", HealthStatus.Degraded);
builder.AddAndConfigureOpenTelemetryTracing();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnSigningOut = async e => { await e.HttpContext.RevokeUserRefreshTokenAsync(); };
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.BackchannelHttpHandler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            SslProtocols = SslProtocols.Tls12,
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        options.Events.OnRedirectToIdentityProvider = context =>
        {
            context.ProtocolMessage.IssuerAddress = builder.Configuration["IdentityServer:IssuerLoginAddress"] ?? throw new Exception("IdentityServer:IssuerLoginAddress configuration not found");
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
        {
            context.ProtocolMessage.IssuerAddress = builder.Configuration["IdentityServer:IssuerLogoutAddress"] ?? throw new Exception("IdentityServer:IssuerLogoutAddress configuration not found");
            return Task.CompletedTask;
        };
        options.RequireHttpsMetadata = false; // Dev channel doesnt require HTTPS
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? throw new Exception("IdentityServer:Authority configuration not found");
        options.ClientId = builder.Configuration["IdentityServer:ClientId"] ?? throw new Exception("IdentityServer:ClientId configuration not found");
        options.ClientSecret = builder.Configuration["IdentityServer:ClientSecret"] ?? throw new Exception("IdentityServer:ClientSecret configuration not found");
        options.ResponseType = "code id_token";

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("address");
        options.Scope.Add("email");
        options.Scope.Add("roles");
        options.Scope.Add("offline_access");
        options.Scope.Add("api_rwx"); // Claim for read write change API resources

        options.ClaimActions.MapUniqueJsonKey("role", "role");

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new()
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role,
            ValidateLifetime = true,
            RequireExpirationTime = true,
        };

        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAccessTokenManagement()
    .ConfigureBackchannelHttpClient()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var baseAddress = builder.Configuration["ApiSettings:GatewayAddress"] ?? throw new Exception("GatewayAddress not configured!");
builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddUserAccessTokenHandler()
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddUserAccessTokenHandler()
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(baseAddress))
    .AddUserAccessTokenHandler()
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IdentityServer:Authority"] ?? throw new Exception("IdentityServer:Authority configuration is missing"));
    client.DefaultRequestHeaders.Clear();
    //client.DefaultRequestHeaders.Accept.Add(new("application/json"));
    client.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
IdentityModelEventSource.ShowPII = true;

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHealthChecks("/hc", new()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
