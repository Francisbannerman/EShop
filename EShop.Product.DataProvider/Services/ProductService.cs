using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;
using EShop.Product.DataProvider.Repositories;

namespace EShop.Product.DataProvider.Services;

public class ProductService : IProductService
{
    private IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<ProductCreated> AddProduct(CreateProduct product)
    {
        //product.ProductId = Guid.NewGuid();
        return await _repository.AddProduct(product);
    }
    
    public async Task<ProductCreated> GetProduct(string ProductId)
    {
        return await _repository.GetProduct(ProductId);
    }
}