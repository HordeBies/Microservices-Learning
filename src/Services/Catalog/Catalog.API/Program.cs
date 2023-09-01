using Catalog.API.Controllers;
using Catalog.DataAccess.DbContext;
using Catalog.DataAccess.Repositories;
using Catalog.Utility;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);
builder.Services.AddHealthChecks()
    .AddMongoDb(builder.Configuration["MongoDbOptions:ConnectionString"]?? throw new Exception("MongoDb Connection String is not configured"), "mongodb", Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
builder.AddAndConfigureOpenTelemetryTracing();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1"
    });
});
builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSingleton<ICatalogContext, CatalogContext>(); //https://mongodb.github.io/mongo-csharp-driver/2.14/reference/driver/connecting/#re-use
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDbOptions"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1");
    });
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new()
{
    Predicate = _ => true,
    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
