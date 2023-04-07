using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Xml.Linq;

namespace Identity.Repository
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class MongoXmlRepository : IXmlRepository
    {
        private readonly IMongoCollection<BsonDocument> collection;
        private readonly string applicationName;

        public MongoXmlRepository(IMongoCollection<BsonDocument> collection, string applicationName)
        {
            this.collection = collection;
            this.applicationName = applicationName;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("ApplicationName", applicationName);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var documents = collection.Find(filter).Project(projection).ToList();

            var elements = documents.Select(d => XElement.Parse(d.GetValue("Xml").AsString)).ToList();

            return elements;
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("FriendlyName", friendlyName),
                Builders<BsonDocument>.Filter.Eq("ApplicationName", applicationName)
            );
            var document = collection.Find(filter).FirstOrDefault();

            if (document == null)
            {
                document = new BsonDocument
            {
                { "FriendlyName", friendlyName },
                { "ApplicationName", applicationName },
                { "Xml", element.ToString() }
            };
                collection.InsertOne(document);
            }
            else
            {
                document["Xml"] = element.ToString();
                collection.ReplaceOne(filter, document);
            }
        }
    }
}
