using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"] ?? throw new Exception("CatalogUrl configuration not found"));
});
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"] ?? throw new Exception("BasketUrl configuration not found"));
});
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"] ?? throw new Exception("OrderingUrl configuration not found"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
