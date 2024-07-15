using EShop.Infrastructure.Event.Product;
using EShop.Infrastructure.Query.Product;
using EShop.Product.DataProvider.Services;
using MassTransit;

namespace EShop.Product.Query.Api.Handlers;

public class GetProductByIdHandler : IConsumer<GetProductById>
{
    private IProductService _service;
    public GetProductByIdHandler(IProductService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<GetProductById> context)
    {
        var product = await _service.GetProduct(context.Message.ProductId);
        await context.RespondAsync<ProductCreated>(product);
    }
}