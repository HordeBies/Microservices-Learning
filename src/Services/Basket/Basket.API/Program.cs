using Basket.DataAccess.Repositories;
using Basket.Services;
using Basket.Utility.Mappings;
using Common.Logging;
using Discount.GRPC.Protos;
using Grpc.Health.V1;
using Grpc.Net.Client;
using MassTransit;
using MassTransit.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);
// Add services to the container.
builder.Services.AddHealthChecks().AddRedis(builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Redis connection string not found"), "redis", HealthStatus.Unhealthy).AddAsyncCheck("discount-grpc", async () =>
    {
        var channel = GrpcChannel.ForAddress(builder.Configuration.GetConnectionString("DiscountGrpc") ?? throw new Exception("DiscountGrpc connection string not found"));
        var client = new Health.HealthClient(channel);

        var response = await client.CheckAsync(new HealthCheckRequest());
        var status = response.Status;
        
        return status == HealthCheckResponse.Types.ServingStatus.Serving ? HealthCheckResult.Healthy(): HealthCheckResult.Unhealthy("DiscountGrpc Server is not healthy!");
    });
builder.AddAndConfigureOpenTelemetryTracing(trace => trace.AddSource(DiagnosticHeaders.DefaultListenerName));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Basket API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
        "Example: \"Bearer 45as678qw12eas\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
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
// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ms-basket_";
});

// General
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// gRPC
builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(o => o.Address = new(builder.Configuration.GetConnectionString("DiscountGrpc") ?? throw new Exception("DiscountGrpc connection string not found"))); // TODO: Add Security to gRPC calls
builder.Services.AddScoped<IDiscountGrpcService, DiscountGrpcService>();

// MassTransit-RabbitMQ
builder.Services.AddMassTransit(c =>
{
    c.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string not found"));
    });
});

// Identity Server
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false; // Dev channel doesnt require HTTPS
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? throw new Exception("IdentityServer:Authority configuration not found");
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiAccessPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api_rwx");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("ApiAccessPolicy");
app.MapHealthChecks("/hc", new()
{
    Predicate = _ => true,
    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
});

// TODO: Add MassTransit-RabbitMQ to listen product price/stock changed events to update basket items preferably async/background job
app.Run();