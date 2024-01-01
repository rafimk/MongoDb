using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDb.Entities;

namespace MongoDb.MongoDb;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("mongo");
        var options = section.BindOptions<MongoOptions>();
        services.Configure<MongoOptions>(section);
        if (!section.Exists())
        {
            return services;
        }

        var mongoClient = new MongoClient(options.ConnectionString);
        var database = mongoClient.GetDatabase(options.Database);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton(database);

        RegisterConventions();

        services.AddSingleton<IRepository<Item>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<Item>(database, "item");
        });

        return services;
    }
    
    private static void RegisterConventions()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(typeof(decimal?),
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
        ConventionRegistry.Register("convey", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
        }, _ => true);
    }
    
    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }
}