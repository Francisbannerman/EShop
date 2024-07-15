using System.Reflection.Metadata;

namespace EShop.Infrastructure.Mongo;

public class MongoConfig
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
}