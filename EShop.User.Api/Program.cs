using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Infrastructure.Security;
using EShop.User.Api.Handlers;
using EShop.User.DataProvider.Repositories;
using EShop.User.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddSingleton<IEncrypter, Encrypter>();

var rabbitmqOption = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqOption);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateUserHandler>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqOption.ConnectionString), hostconfig =>
        {
            hostconfig.Username(rabbitmqOption.Username);
            hostconfig.Password(rabbitmqOption.Password);
        });
        cfg.ReceiveEndpoint("add_user", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<CreateUserHandler>(provider);
        });
    }));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();