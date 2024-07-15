using EShop.Cart.DataProvider.Repository;

namespace EShop.Cart.DataProvider.Services;

using EShop.Infrastructure.Cart;
public class CartService : ICartService
{
    private ICartRepository _cartRepository;
    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }
    
    public async Task<bool> AddCart(Cart cart)
    {
        return await _cartRepository.AddCart(cart);
    }

    public async Task<Cart> GetCart(string UserId)
    {
        return await _cartRepository.GetCart(UserId);
    }

    public async Task<bool> RemoveCart(string UserId)
    {
        return await _cartRepository.RemoveCart(UserId);
    }
}