using CommentWebAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CommentWebAPI.Services
{
    public interface IRepositoryComments
    {
        Task<List<Comment>> GetAsync();
        Task<Comment?> GetAsync(string id);
        Task<List<Comment>> GetTopVotesAsync(int number,int limit);
        Task CreateAsync(Comment newComment);
        Task UpdateAsync(string id, Comment updateComment);
        }
    }