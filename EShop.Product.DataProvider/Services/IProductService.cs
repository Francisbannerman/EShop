using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;

namespace EShop.Product.DataProvider.Services;

public interface IProductService
{
    Task<ProductCreated> GetProduct(string ProductId);
    Task<ProductCreated> AddProduct(CreateProduct product);
}