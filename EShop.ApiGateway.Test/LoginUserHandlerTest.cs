using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using EShop.Infrastructure.Security;
using EShop.User.DataProvider.Services;
using EShop.User.Query.Api.Handlers;
using MassTransit.Testing;
using Moq;

namespace EShop.ApiGateway.Test;

[TestFixture]
public class LoginUserHandlerTest
{
    [Test(TestOf = typeof(LoginUserHandler))]
    public async Task LoginUserReturnsToken()
    {
        LoginUser loginUserMock = new LoginUser()
        {
            Username = "john_doe", Password = "john_sus"
        };
        UserCreated userCreatedMock = new UserCreated()
        {
            Username = "john_doe", Password = "john_sus",
            ContactNo = "+233267067953", UserId = "john_doe",
            EmailId = "john@cv.com"
        };
        JwtAuthToken token = new JwtAuthToken()
        {
            Token = "53hsdhsk474ksjsh7hdkkf?==", Expires = default(long)
        };
        var harness = new InMemoryTestHarness();
        
        try
        {
            //Arrange
            var service = new Mock<IUserService>();
            service.Setup(x => x.GetUserByUsername
                (It.IsAny<string>())).ReturnsAsync(userCreatedMock);

            var authHandler = new Mock<IAuthenticationHandler>();
            authHandler.Setup(x =>
                x.Create(It.IsAny<string>())).Returns(token);

            var encrypter = new Mock<IEncrypter>();
            encrypter.Setup(e => e.GetHash
                (It.IsAny<string>(), It.IsAny<string>())).Returns
                (userCreatedMock.Password);

            var consumerHarness = harness.Consumer(() => new LoginUserHandler
                (service.Object, encrypter.Object, authHandler.Object));

            //Act
            await harness.Start();
            var requestClient = harness.CreateRequestClient<LoginUser>();
            var user = await requestClient.
                GetResponse<JwtAuthToken>(loginUserMock);
            
            //Assert
            Assert.That(user.Message.Token == token.Token);
        }
        finally
        {
            harness.Stop();
        }
    }
}