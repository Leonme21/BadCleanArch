using Application.UseCases;
using Domain.Services;
using Infrastructure.Logging;
using Infrastructure.Repositories;
using DomainLogger = Domain.Interfaces.ILogger;
using DomainOrderRepository = Domain.Interfaces.IOrderRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigins", policy => 
{
    policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
          .AllowAnyHeader()
          .AllowAnyMethod();
}));

var connectionString = builder.Configuration.GetConnectionString("Sql") 
    ?? builder.Configuration["ConnectionStrings:Sql"]
    ?? throw new InvalidOperationException("Database connection string is required");

// Replace environment variable placeholder with actual value
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_PASSWORD")))
{
    connectionString = connectionString.Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD")!);
}

builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<DomainLogger>(new ConsoleLogger(isEnabled: true));
builder.Services.AddSingleton<DomainOrderRepository>(sp => 
    new OrderRepository(connectionString, sp.GetRequiredService<DomainLogger>()));
builder.Services.AddScoped<CreateOrderUseCase>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var logger = context.RequestServices.GetRequiredService<DomainLogger>();
        logger.LogError("Unhandled exception occurred", null);
        
        await context.Response.WriteAsJsonAsync(new { error = "An error occurred processing your request" });
    });
});

app.MapGet("/health", (DomainLogger logger) =>
{
    logger.LogInformation("Health check requested");
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
});

app.MapPost("/orders", async (CreateOrderRequest request, CreateOrderUseCase useCase, DomainLogger logger) =>
{
    try
    {
        if (request == null)
        {
            return Results.BadRequest(new { error = "Request body is required" });
        }

        var order = await useCase.ExecuteAsync(
            request.CustomerName, 
            request.ProductName, 
            request.Quantity, 
            request.UnitPrice);

        return Results.Ok(new 
        { 
            id = order.Id,
            customerName = order.CustomerName,
            productName = order.ProductName,
            quantity = order.Quantity,
            unitPrice = order.UnitPrice,
            total = order.CalculateTotal()
        });
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning($"Invalid order request: {ex.Message}");
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        logger.LogError("Error creating order", ex);
        return Results.Problem("An error occurred while creating the order");
    }
});

app.MapGet("/orders", async (DomainOrderRepository repository, DomainLogger logger) =>
{
    try
    {
        var orders = await repository.GetAllAsync();
        return Results.Ok(orders);
    }
    catch (Exception ex)
    {
        logger.LogError("Error retrieving orders", ex);
        return Results.Problem("An error occurred while retrieving orders");
    }
});

app.MapGet("/info", () => new
{
    version = "v1.0.0",
    environment = builder.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow
});

await app.RunAsync();

#pragma warning disable S3903
public record CreateOrderRequest(
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
#pragma warning restore S3903
