using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelprAPI.Models;
using HelprAPI.Utility;

namespace HelprAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        //fields for accessing database connection and querying the datbase
        private AppDb Db;
        private PostDbQuery PostQuery;
        private AuthorizationQuery AuthorizationQuery;
        public PostsController(AppDb db)
        {
            Db = db;
            PostQuery = new PostDbQuery(Db);
            AuthorizationQuery = new AuthorizationQuery(Db);
        }


        //API Calls:

        //POST /api/posts/add/thread
        [HttpPost ("add/thread")]
        public async Task<IActionResult> PostNewThread([FromBody] ThreadModel body, [FromHeader] string token)
        {
            await Db.Connection.OpenAsync();
            
            //check that user is logged in
            if(await AuthorizationQuery.GetTokenModel(token) != null)
            {
                //return 200 if thread is successfully added
                if (await PostQuery.AddThread(body))
                {
                    return new OkResult();
                }

                return new NotFoundObjectResult("Could not add thread");
            }

            return new NotFoundObjectResult("User must be logged in to add thread");
        }

        //GET /api/posts/threads
        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads()
        {
            await Db.Connection.OpenAsync();
            
            return new OkObjectResult(await PostQuery.GetThreads());
        }

        //POST /api/posts/add/post
        [HttpPost("add/post")]
        public async Task<IActionResult> PostPost([FromBody] PostModel body, [FromHeader] string token)
        {
            await Db.Connection.OpenAsync();

            //check that user is logged in
            if (await AuthorizationQuery.GetTokenModel(token) != null)
            {
                if (await PostQuery.AddPost(body))
                {
                    return new OkResult();
                }
                else
                {
                    return new NotFoundObjectResult("Could not add post");
                }
            }

            return new NotFoundObjectResult("User must be logged in to create post");


                                    //TODO: authorize thread_id and get user_id from token instead of body
        }

        //GET /api/posts/posts
        [HttpGet ("posts")]
        public async Task<IActionResult> GetPosts([FromBody] ThreadModel body, [FromHeader] string token)
        {
            await Db.Connection.OpenAsync();

            //check that user is logged in
            if (await AuthorizationQuery.GetTokenModel(token) != null)
            {
                return new OkObjectResult(await PostQuery.GetPosts(body));
            }

            return new NotFoundObjectResult("User must be logged in to view posts");
        }

    }
}