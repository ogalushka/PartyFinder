using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Common.Settings;
using MongoDB.Driver;

namespace Common.Mongo
{
    public static class MongoSetup
    {
        public static void SetupDb(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton(sp => {
                var config = sp.GetService<IConfiguration>();
                if (config == null)
                {
                    throw new Exception("Can't resolve IConfiguration from service provider");
                }

                var mongoSettings = config.GetSection(nameof(MongoDbSettings))?.Get<MongoDbSettings>();
                var serviceSettings = config.GetSection(nameof(ServiceSettings))?.Get<ServiceSettings>();
                if (mongoSettings == null || serviceSettings == null)
                {
                    throw new Exception("MongoDbSettings or ServiceSettings section missing from configuration");
                }

                var mongoClient = new MongoClient(mongoSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
        }

        public static void SetupRepository<TKey, TValue>(IServiceCollection services, string collectionName) where TValue : IEntity<TKey>
        {
            services.AddSingleton<IRepository<TKey, TValue>>(sp => {
                var database = sp.GetService<IMongoDatabase>();
                if (database == null)
                {
                    throw new Exception("Failed to resolve database from service provider");
                }
                return new MongoRepository<TKey, TValue>(database, collectionName);
            });
        }
    }
}
