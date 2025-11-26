using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    Task<bool> SaveAsync(Order order);
    Task<IEnumerable<Order>> GetAllAsync();
}
