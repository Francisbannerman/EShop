using EShop.Infrastructure.Command.User;
using EShop.User.DataProvider.Services;
using MassTransit;

namespace EShop.User.Api.Handlers;

public class CreateUserHandler : IConsumer<CreateUser>
{
    private IUserService _service;
    public CreateUserHandler(IUserService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var createdUser = await _service.AddUser(context.Message);
    }
}