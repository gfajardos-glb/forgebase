using MongoDB.Bson;
using MongoDB.Driver;

namespace ForgeBase.Mongo
{
    public interface IMongoDataAccess
    {
        IMongoCollection<BsonDocument> GetCollection(string collectionName);
    }
}