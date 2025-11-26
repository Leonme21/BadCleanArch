using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public OrderRepository(string connectionString, ILogger logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SaveAsync(Order order)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO Orders (Id, CustomerName, ProductName, Quantity, UnitPrice) 
                VALUES (@Id, @CustomerName, @ProductName, @Quantity, @UnitPrice)";

            using var command = new SqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
            command.Parameters.AddWithValue("@ProductName", order.ProductName);
            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);

            await command.ExecuteNonQueryAsync();
            _logger.LogInformation($"Order {order.Id} saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save order {order.Id}", ex);
            return false;
        }
    }

    /// <summary>
    /// Retrieves all orders from the database.
    /// </summary>
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var orders = new List<Order>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT Id, CustomerName, ProductName, Quantity, UnitPrice FROM Orders";
            
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new Order
                {
                    Id = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    ProductName = reader.GetString(2),
                    Quantity = reader.GetInt32(3),
                    UnitPrice = reader.GetDecimal(4)
                });
            }

            _logger.LogInformation($"Retrieved {orders.Count} orders from database");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to retrieve orders", ex);
        }

        return orders;
    }
}
