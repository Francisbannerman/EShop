using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Product.Api.Handlers;
using EShop.Product.DataProvider.Repositories;
using EShop.Product.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<CreateProductHandler>();

var rabbitmqOption = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqOption);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateProductHandler>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqOption.ConnectionString), hostconfig =>
        {
            hostconfig.Username(rabbitmqOption.Username);
            hostconfig.Password(rabbitmqOption.Password);
        });
        cfg.ReceiveEndpoint("create_product", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<CreateProductHandler>(provider);
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

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();