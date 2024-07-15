using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Infrastructure.Security;
using EShop.User.DataProvider.Repositories;
using EShop.User.DataProvider.Services;
using EShop.User.Query.Api.Handlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddSingleton<IEncrypter, Encrypter>();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<IAuthenticationHandler, AuthenticationHandler>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LoginUserHandler>();
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
var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();