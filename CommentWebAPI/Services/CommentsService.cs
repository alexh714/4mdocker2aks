using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CommentWebAPI.Models;
using MongoDB.Driver.Linq;
namespace CommentWebAPI.Services
{
    public class CommentsService: IRepositoryComments
    {
        private readonly IMongoCollection<Comment> _commentsCollection;
        public CommentsService(IOptions<DatabaseSettings> commentDatabaseSettings) 
        {
            var mongoClient = new MongoClient(commentDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(commentDatabaseSettings.Value.DatabaseName);

            _commentsCollection = mongoDatabase.GetCollection<Comment>(commentDatabaseSettings.Value.CommentsCollectionName);
        }
        public async Task<List<Comment>> GetAsync()
        {
            List<Comment> comments = new List<Comment>();
            var emptyFilter = Builders<Comment>.Filter.Empty;
            comments = await _commentsCollection.Find(emptyFilter).ToListAsync();
            return comments;
        }
        public async Task<Comment?> GetAsync (string id)
        {
            var comment = await _commentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return comment;
        }
        public async Task<List<Comment>> GetTopVotesAsync(int number, int limit)
        {
            List<Comment> comments = new List<Comment>();
            var queryableComments = _commentsCollection.AsQueryable();
            comments = await queryableComments.OrderBy(r => r.Votes).Take(limit).ToListAsync();
            return comments;
        }
        public async Task CreateAsync(Comment newComment)
        {
            await _commentsCollection.InsertOneAsync(newComment);
        }
        public async Task UpdateAsync(string id, Comment updateComment)
        {
            await _commentsCollection.ReplaceOneAsync(x => x.Id == id, updateComment);
        }
    }
}
