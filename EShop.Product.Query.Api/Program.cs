using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Product.DataProvider.Repositories;
using EShop.Product.DataProvider.Services;
using EShop.Product.Query.Api.Handlers;
using MassTransit;
using MongoDB.Bson.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GetProductByIdHandler>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var rabbitMq = new RabbitMqOption();
        builder.Configuration.GetSection("rabbitmq").Bind(rabbitMq);
        cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg =>
        {
            hostcfg.Username(rabbitMq.Username);
            hostcfg.Password(rabbitMq.Password);
        });
        cfg.ConfigureEndpoints(provider);
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
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
var bus = app.Services.GetService<IBusControl>();
bus.Start();
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();


app.Run();