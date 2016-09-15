using ForgeBase.Mongo;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeBase.Routing
{
    public class RouteHandler
    {
        private readonly IMongoDataAccess _database;

        /****************************************
         *              Routes                  *
         ****************************************
         *              
         * GET  /collection
         * GET  /collection/{id}
         * GET  /collection/{id}/property
         *
         * POST /collection
         * PUT  /collection/{id}
         * PUT  /collection/{id}/property
         * 
         * DELETE /collection
         * DELETE //collection/{id}
         * 
         ***************************************/

        public RouteHandler(IMongoDataAccess database)
        {
            _database = database;
        }

        public dynamic ProcessRoute(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            var route = path.Value.Trim().Split('/');

            var collection = route.Length > 1 ? route[1] : null;
            var id = route.Length > 2 ? route[2] : null;
            var property = route.Length > 3 ? route[3] : null;

            Stream req = context.Request.Body;
            string json = new StreamReader(req).ReadToEnd();

            string response = string.Format("<b>{0}</b> {1} </br><b>Collection:</b> {2} </br><b>Id:</b> {3} </br><b>Property:</b> {4} </br><b>Body:</b> {5} ",
                method, path, collection, id, property, json);
            switch (method.ToLower())
            {
                case "get":
                    response = Get(collection, id, property);
                    break;
                case "post":
                    response = Post(collection, id, property, json);
                    break;
                case "put":

                    break;
                case "delete":

                    break;
                default:
                    break;
            }

            return response;
        }

        private string Post(string collection, string id, string property, string json)
        {
            try
            {
                var doc = BsonSerializer.Deserialize<BsonDocument>(json);

                _database.GetCollection(collection).InsertOne(doc);

                return doc.ToJson();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string Get(string collection, string id, string property)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    var doc = _database.GetCollection(collection);
                    
                    return doc.Find(FilterDefinition<BsonDocument>.Empty).ToList().ToJson();
                }
                else
                {

                    var builder = Builders<BsonDocument>.Filter;
                    var filter = builder.Eq("_id", ObjectId.Parse(id));

                    var doc = _database.GetCollection(collection).FindSync(filter);

                    if (string.IsNullOrEmpty(property))
                    {
                        return doc.FirstOrDefault().ToJson();
                    }


                    return doc.FirstOrDefault()[property].ToJson();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
