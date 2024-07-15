using EShop.Infrastructure.Command.Product;
using EShop.Product.DataProvider.Services;
using MassTransit;

namespace EShop.Product.Api.Handlers;

public class CreateProductHandler : IConsumer<CreateProduct>
{
    private IProductService _service;
    public CreateProductHandler(IProductService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<CreateProduct> context)
    {
        await _service.AddProduct(context.Message);
        await Task.CompletedTask;
    }
}