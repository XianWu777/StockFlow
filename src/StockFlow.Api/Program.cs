using StockFlow.Application.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// customize the application configuration
builder.Services.AddControllers();
// 有transient, scoped, singleton分別
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// customize the application configuration
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();