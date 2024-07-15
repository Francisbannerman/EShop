using EShop.Infrastructure.Command.Product;
using EShop.Product.DataProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Product.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    public IProductService _service { get; set; }
    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string productId)
    {
        var product = await _service.GetProduct(productId);
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromForm] CreateProduct product)
    {
        var addedProduct = await _service.AddProduct(product);
        return Ok(addedProduct);
    }
}