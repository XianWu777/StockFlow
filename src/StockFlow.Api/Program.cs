using Microsoft.EntityFrameworkCore;
using StockFlow.Api.Middleware;
using StockFlow.Application.Products;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Entity;
using StockFlow.Infrastructure.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// customize the application configuration
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 有transient, scoped, singleton分別
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddDbContext<StockFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// customize the application configuration
// app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();