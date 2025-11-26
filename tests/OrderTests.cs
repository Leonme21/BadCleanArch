using System;
using Xunit;
using Domain.Entities;
using Domain.Services;

namespace Tests;

public class OrderTests
{
    [Fact]
    public void Order_CalculateTotal_ReturnsCorrectSum()
    {
        var order = new Order 
        { 
            Id = 1,
            CustomerName = "John",
            ProductName = "Widget",
            Quantity = 5,
            UnitPrice = 10.0m
        };

        var total = order.CalculateTotal();

        Assert.Equal(50.0m, total);
    }

    [Fact]
    public void Order_IsValid_ReturnsTrueForValidOrder()
    {
        var order = new Order 
        { 
            Id = 1,
            CustomerName = "John",
            ProductName = "Widget",
            Quantity = 5,
            UnitPrice = 10.0m
        };

        var isValid = order.IsValid();

        Assert.True(isValid);
    }

    [Fact]
    public void Order_IsValid_ReturnsFalseForEmptyCustomerName()
    {
        var order = new Order 
        { 
            Id = 1,
            CustomerName = "",
            ProductName = "Widget",
            Quantity = 5,
            UnitPrice = 10.0m
        };

        var isValid = order.IsValid();

        Assert.False(isValid);
    }

    [Fact]
    public void Order_IsValid_ReturnsFalseForZeroQuantity()
    {
        var order = new Order 
        { 
            Id = 1,
            CustomerName = "John",
            ProductName = "Widget",
            Quantity = 0,
            UnitPrice = 10.0m
        };

        var isValid = order.IsValid();

        Assert.False(isValid);
    }

    [Fact]
    public void OrderService_CreateOrder_ReturnsValidOrder()
    {
        var service = new OrderService();

        var order = service.CreateOrder("John", "Widget", 5, 10.0m);

        Assert.NotNull(order);
        Assert.Equal("John", order.CustomerName);
        Assert.Equal("Widget", order.ProductName);
        Assert.Equal(5, order.Quantity);
        Assert.Equal(10.0m, order.UnitPrice);
    }

    [Fact]
    public void OrderService_CreateOrder_ThrowsExceptionForInvalidData()
    {
        var service = new OrderService();

        var exception = Assert.Throws<ArgumentException>(
            () => service.CreateOrder("", "Widget", 5, 10.0m)
        );

        Assert.NotNull(exception);
    }
}
