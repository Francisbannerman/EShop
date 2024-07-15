namespace EShop.Order.DataProvider.Repository;

using EShop.Infrastructure.Order;
public interface IOrderRepository
{
    Task<Order> GetOrder(string OrderId);
    Task<List<Order>> GetAllOrders(string UserId);
    Task<bool> CreateOrder(Order order);
}