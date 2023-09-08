using Common.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddAuthentication()
    .AddJwtBearer("IdentityApiKey", options =>
    {
        options.RequireHttpsMetadata = false; // Dev channel doesnt require HTTPS
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? throw new Exception("IdentityServer:Authority configuration not found");
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddOcelot(builder.Configuration).AddCacheManager(opt => opt.WithDictionaryHandle());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapControllers());
//app.MapControllers();

await app.UseOcelot();

app.Run();