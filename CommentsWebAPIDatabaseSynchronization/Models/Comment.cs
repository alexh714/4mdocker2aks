using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommentsWebAPIDatabaseSynchronization.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("postId")]
        public int PostId { get; set; }
        [BsonElement("parentId")]
        public int ParentId { get; set; }
        [BsonElement("votes")]
        public int Votes { get; set; }
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("body")]
        public string Body { get; set; } = string.Empty;
    }
}
