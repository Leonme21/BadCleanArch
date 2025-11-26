using System;
using System.Collections.Generic;

namespace Domain.Services;

using Domain.Entities;

public class OrderService
{
    public Order CreateOrder(string customer, string product, int quantity, decimal unitPrice)
    {
        var order = new Order 
        { 
            Id = GenerateOrderId(), 
            CustomerName = customer, 
            ProductName = product, 
            Quantity = quantity, 
            UnitPrice = unitPrice 
        };

        if (!order.IsValid())
        {
            throw new ArgumentException("Invalid order data provided");
        }

        return order;
    }

    private static int GenerateOrderId()
    {
        return (int)(DateTime.UtcNow.Ticks % int.MaxValue);
    }
}
