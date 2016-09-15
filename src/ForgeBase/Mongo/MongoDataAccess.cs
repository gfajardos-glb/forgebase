using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeBase.Mongo
{
    public class MongoDataAccess : IMongoDataAccess
    {
        private readonly IMongoDatabase _database;

        public MongoDataAccess()
        {
            MongoClient server = new MongoClient("mongodb://localhost");
            _database = server.GetDatabase("forgebase");
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("missing collectionName");
            return _database.GetCollection<BsonDocument>(collectionName);
        }

    }
}
