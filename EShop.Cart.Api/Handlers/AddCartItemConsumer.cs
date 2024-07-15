
namespace EShop.Cart.Api.Handlers;

using MassTransit;
using EShop.Cart.DataProvider.Services;
using EShop.Infrastructure.Cart;

public class AddCartItemConsumer : IConsumer<Cart>
{
    private ICartService _cartService;
    public AddCartItemConsumer(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task Consume(ConsumeContext<Cart> context)
    {
        await _cartService.AddCart(context.Message);
        await Task.CompletedTask;
    }
}