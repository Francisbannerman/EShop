using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace EShop.Infrastructure.Mongo;

public static class Extension
{
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var configSection = configuration.GetSection("mongo");

        var mongoConfig = new MongoConfig();
        configSection.Bind(mongoConfig);

        services.AddSingleton<IMongoClient>(client =>
        {
            return new MongoClient(mongoConfig.ConnectionString);
        });
        services.AddSingleton<IMongoDatabase>(client =>
        {
            var mongoClient = client.GetService<IMongoClient>();
            return mongoClient.GetDatabase(mongoConfig.Database);
        });
        services.AddSingleton<IDatabaseInitializer, MongoInitializer>();
    }
}