using StackExchange.Redis;
using MongoDB.Driver;
using CommentsWebAPIDatabaseSynchronization.Models;
using System.Text.Json;
using MongoDB.Bson;
namespace CommentWebAPIDatabaseSynchronization
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("starting...\n");
            string ConnectionString = "mongodb://mongo:27017";
            var mongoClient = new MongoClient(ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("Popularity");
            IMongoCollection<Comment> commentsCollection = mongoDatabase.GetCollection<Comment>("Comments");

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis:6379");
            IDatabase db = redis.GetDatabase();

            foreach (var k in redis.GetServer("redis",6379).Keys()) 
            {
                string key = k.ToString();
                var value = await db.StringGetAsync(key);
                Comment comment = JsonSerializer.Deserialize<Comment>(value.ToString());
                if (comment != null)
                {       
                    var filter = Builders<Comment>.Filter.Eq("_id", new ObjectId(comment.Id));
                    var res = await commentsCollection.ReplaceOneAsync(filter, comment);
                    Console.WriteLine($"{res} for {comment.Id}");
                }
            }
            //Console.WriteLine("closing");
        }
    }
}
