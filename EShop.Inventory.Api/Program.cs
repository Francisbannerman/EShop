using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Inventory.Api.Handlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitmqOption = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqOption);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AllocateProductConsumer>();
    x.AddConsumer<ReleaseProductConsumer>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqOption.ConnectionString), hostconfig =>
        {
            hostconfig.Username(rabbitmqOption.Username);
            hostconfig.Password(rabbitmqOption.Password);
        });
        cfg.ReceiveEndpoint("allocate_product", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<AllocateProductConsumer>(provider);
        });
        cfg.ReceiveEndpoint("release_product", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<ReleaseProductConsumer>(provider);
        });
    }));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();

app.Run();