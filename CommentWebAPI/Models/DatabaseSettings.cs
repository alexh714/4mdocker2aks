namespace CommentWebAPI.Models
{
    public class DatabaseSettings
    {
        public DatabaseSettings() { }
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CommentsCollectionName {  get; set; } = string.Empty;
    }
}
