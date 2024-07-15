using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Wallet.Api.Handlers;
using EShop.Wallet.DataProvider.Repository;
using EShop.Wallet.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddMongoDb(builder.Configuration);
var rabbitmqConfig = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqConfig);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AddFundsConsumer>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqConfig.ConnectionString), hostConfig =>
        {
            hostConfig.Username(rabbitmqConfig.Username);
            hostConfig.Password(rabbitmqConfig.Password);
        });
        cfg.ReceiveEndpoint("add_funds", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig =>
            {
                retryConfig.Interval(2, 100);
            });
            ep.ConfigureConsumer<AddFundsConsumer>(provider);
        });
        cfg.ReceiveEndpoint("deduct_funds", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig =>
            {
                retryConfig.Interval(2, 100);
            });
            ep.ConfigureConsumer<DeductFundsConsumer>(provider);
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

app.UseEndpoints(endPoints =>
{
    endPoints.MapControllers();
});
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();


app.Run();