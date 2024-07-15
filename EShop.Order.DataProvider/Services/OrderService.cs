using EShop.Order.DataProvider.Repository;

namespace EShop.Order.DataProvider.Services;

using EShop.Infrastructure.Order;
using System.Threading.Tasks;
public class OrderService : IOrderService
{
    private IOrderRepository _orderRepository;
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<Order> GetOrder(string OrderId)
    {
        return await _orderRepository.GetOrder(OrderId);
    }

    public async Task<List<Order>> GetAllOrders(string UserId)
    {
        return await _orderRepository.GetAllOrders(UserId);
    }

    public async Task<bool> CreateOrder(Order order)
    {
        return await _orderRepository.CreateOrder(order);
    }
}