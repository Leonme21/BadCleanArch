using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal CalculateTotal()
    {
        return Quantity * UnitPrice;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(CustomerName) 
            && !string.IsNullOrWhiteSpace(ProductName)
            && Quantity > 0
            && UnitPrice > 0;
    }
}
