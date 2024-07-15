using EShop.Infrastructure.Mongo;
using EShop.Infrastructure.EventBus;
using EShop.Order.Api.Handlers;
using EShop.Order.DataProvider.Repository;
using EShop.Order.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1", Title = "EShop Order API Endpoints",
        Description = "These API Endpoints are available to CRUD order related data"
    });
});

var rabbitmqOption = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqOption);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateOrderHandler>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqOption.ConnectionString), hostconfig =>
        {
            hostconfig.Username(rabbitmqOption.Username);
            hostconfig.Password(rabbitmqOption.Password);
        });
        cfg.ReceiveEndpoint("create_order", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<CreateOrderHandler>(provider);
        });
    }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
});

app.Run();