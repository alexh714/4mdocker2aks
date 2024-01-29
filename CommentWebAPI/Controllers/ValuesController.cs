using CommentWebAPI.Services;
using CommentWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using StackExchange.Redis;
using System.Text.Json.Nodes;

namespace CommentWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentsService _commentsService;
        private readonly IConnectionMultiplexer _multiplexer;
        public CommentsController(CommentsService commentsService, IConnectionMultiplexer multiplexer) 
        {
            this._commentsService = commentsService;
            this._multiplexer = multiplexer;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<List<Comment>> Get()
        {
            //return new string[] { "value1", "value2" };
            return await _commentsService.GetAsync();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> Get(string id)
        {
            IDatabase redisDB = this._multiplexer.GetDatabase();
            var res = await redisDB.StringGetAsync(id);
            if (res.ToString().Length > 0)
            {
                Comment com = JsonSerializer.Deserialize<Comment>(res.ToString());
                return Ok(com);
            }
            else
            {
                Console.WriteLine($"id {id} not found in redis\nchecking in mongodb");
                var result = await _commentsService.GetAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                await redisDB.StringSetAsync(result.Id, JsonSerializer.Serialize(result));
                return Ok(result);
            }
            

        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, Comment value)
        {
            if (value != null && value.Id == id) 
            {
                //Comment updateComment = JsonSerializer.Deserialize<Comment>(value);
                //await _commentsService.UpdateAsync(id, updateComment);
                IDatabase redisDB = this._multiplexer.GetDatabase();
                var stringComment = await redisDB.StringGetAsync(value.Id);
                if(stringComment.ToString().Length > 0)
                {
                    Console.WriteLine("found in redis");
                    Comment updateComment = JsonSerializer.Deserialize<Comment>(stringComment.ToString());
                    if(value.Id == updateComment.Id)
                    {
                    await redisDB.StringSetAsync(updateComment.Id, JsonSerializer.Serialize(value));
                    return Created($"api/Comments{updateComment.Id}", value);
                    }
                }
                else
                {
                    Console.WriteLine($"id {id} not found in redis\nchecking in mongodb");
                    Comment updateComment = await _commentsService.GetAsync(id);
                    if (updateComment == null)
                    {
                        return NotFound();
                    }
                    updateComment.Votes = value.Votes;
                    await redisDB.StringSetAsync(updateComment.Id, JsonSerializer.Serialize(updateComment));
                    return Created($"api/Comments{updateComment.Id}", updateComment);
                }
            }
            return new ContentResult()
            {
                Content = "No modified",
                ContentType = "application/text",
                StatusCode = 304
            };
            //return NotFound();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //return new NotImplementedException();
            return;
        }
        [HttpGet("/hello")]
        public IActionResult Hello()
        {
            string jsonBody = @"{
            ""msg"":""this worked""}";
            return new ContentResult() {
                Content = jsonBody,
                ContentType = "application/json",
                StatusCode = 200             
            };

        }
    }
}
