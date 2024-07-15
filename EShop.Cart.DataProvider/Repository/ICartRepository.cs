namespace EShop.Cart.DataProvider.Repository;

using EShop.Infrastructure.Cart;
public interface ICartRepository
{
    Task<bool> AddCart(Cart cart);
    Task<Cart> GetCart(string UserId);
    Task<bool> RemoveCart(string UserId);
}