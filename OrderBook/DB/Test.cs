using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace OrderBook.DB
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
    internal static class Test
    {
        public static void GetTest()
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var collection = database.GetCollection<Entity>("entities");

            var entity = new Entity { Name = "Tom" };
            collection.Insert(entity);
            var id = entity.Id;

            var query = Query<Entity>.EQ(e => e.Id, id);
            entity = collection.FindOne(query);

            entity.Name = "Dick";
            collection.Save(entity);

            var update = Update<Entity>.Set(e => e.Name, "Harry");
            collection.Update(query, update);

            collection.Remove(query);
        }
    }
}