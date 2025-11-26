using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Services;
using Domain.Interfaces;

namespace Application.UseCases;

public class CreateOrderUseCase
{
    private readonly OrderService _orderService;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public CreateOrderUseCase(
        OrderService orderService,
        IOrderRepository orderRepository,
        ILogger logger)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Order> ExecuteAsync(string customer, string product, int quantity, decimal unitPrice)
    {
        _logger.LogInformation("CreateOrderUseCase starting");

        try
        {
            var order = _orderService.CreateOrder(customer, product, quantity, unitPrice);
            
            _logger.LogInformation($"Order created with ID: {order.Id}");

            var saved = await _orderRepository.SaveAsync(order);
            
            if (!saved)
            {
                _logger.LogWarning($"Order {order.Id} was created but failed to persist");
            }
            else
            {
                _logger.LogInformation($"Order {order.Id} successfully persisted");
            }

            return order;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Invalid order data provided", ex);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating order", ex);
            throw;
        }
    }
}
