using Common.Logging;
using Discount.DataAccess.DbInitializers;
using Discount.DataAccess.Repositories;
using Discount.GRPC.Mapper;
using Discount.GRPC.Services;
using Discount.Utilities;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);
// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks().AddNpgSql(builder.Configuration["DatabaseSettings:PostgreSqlConnectionString"] ?? throw new Exception("DatabaseSettings:PostgreSqlConnectionString configuration not found"), name: "postgres", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
builder.AddAndConfigureOpenTelemetryTracing();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddAutoMapper(typeof(DiscountProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcHealthChecksService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
await InitializeDatabase();

app.Run();

async Task InitializeDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await dbInitializer.Initialize();
    }
}