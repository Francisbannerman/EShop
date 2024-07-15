namespace EShop.Order.DataProvider.Services;

using EShop.Infrastructure.Order;
using System.Threading.Tasks;
public interface IOrderService
{
    Task<Order> GetOrder(string OrderId);
    Task<List<Order>> GetAllOrders(string UserId);
    Task<bool> CreateOrder(Order order);
}