using System.Net;
using System.Runtime.CompilerServices;
using EShop.ApiGateway.Controllers;
using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;
using EShop.Infrastructure.Query.Product;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace EShop.ApiGateway.Test;

public class DerivedClass : Response<ProductCreated>
{
    public Guid? MessageId => throw new NotImplementedException();
    public Guid? RequestId => throw new NotImplementedException();
    public Guid? CorrelationId => throw new NotImplementedException();
    public Guid? ConversationId => throw new NotImplementedException();
    public Guid? InitiatorId => throw new NotImplementedException();
    public DateTime? ExpirationTime => throw new NotImplementedException();
    public Uri? SourceAddress => throw new NotImplementedException();
    public Uri? DestinationAddress => throw new NotImplementedException();
    public Uri? ResponseAddress => throw new NotImplementedException();
    public Uri? FaultAddress => throw new NotImplementedException();
    public DateTime? SentTime => throw new NotImplementedException();
    public Headers Headers => throw new NotImplementedException();
    public HostInfo Host => throw new NotImplementedException();
    public ProductCreated Message => new ProductCreated()
    {
        ProductId = "111", ProductName = "testing product",
        CreatedAt = DateTime.UtcNow
    };
    object Response.Message => Message;
}

public class ProductControllerTest
{
    ProductController controller = null;
    [SetUp]
    public void Setup()
    {
        var sendEndpoint = new Mock<ISendEndpoint>();

        var busControl = new Mock<IBusControl>();
        busControl.Setup(x => x.GetSendEndpoint
            (It.IsAny<Uri>())).ReturnsAsync(sendEndpoint.Object);
        
        var deriveResponse = new Mock<DerivedClass>();
        var requestClient = new Mock<IRequestClient<GetProductById>>();
        requestClient.Setup(x =>
            x.GetResponse<ProductCreated>(It.IsAny<GetProductById>(),
                It.IsAny<CancellationToken>(), It.IsAny
                    <RequestTimeout>())).ReturnsAsync(deriveResponse.Object);
        
        controller = new ProductController(busControl.Object, requestClient.Object);
    }

    [Test]
    public async Task AddProductReturnsAcceptedResult()
    {
        
        var result = await controller.Add(It.IsAny<CreateProduct>());
        
        Assert.That((result as AcceptedResult).StatusCode == (int?)HttpStatusCode.Accepted);
    }

    [Test]
    public async Task GetProductReturnsProductWithAcceptedResult()
    {
        var expectedProduct = new ProductCreated()
        {
            ProductId = "111", ProductName = "testing product",
            CreatedAt = DateTime.UtcNow
        };
        var result = await controller.Get("111");
        
        Assert.That((result as AcceptedResult).StatusCode == (int?)HttpStatusCode.Accepted);
        var receivedProduct = ((result as AcceptedResult).Value as Response<ProductCreated>).Message;
        
        Assert.That(receivedProduct.ProductId == expectedProduct.ProductId);
    }
}