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
        private UserDbQuery userQuery;
        private PostDbQuery postQuery;
        private AuthorizationQuery authorizationQuery;
        public PostsController(AppDb db)
        {
            Db = db;
            userQuery = new UserDbQuery(Db);
            postQuery = new PostDbQuery(Db);
            authorizationQuery = new AuthorizationQuery(Db);
        }


        //API Calls:

        //POST /api/posts/add/thread
        [HttpPost ("add/thread")]
        public async Task<IActionResult> PostNewThread([FromBody] ThreadModel body, [FromHeader] string token)
        {
            if(!body.IsValidThread())
            {
                return new NotFoundObjectResult("Invalid body");
            }
            
            await Db.Connection.OpenAsync();

            //check that user is logged in
            if(await authorizationQuery.GetTokenModel(token) != null)
            {
                //return 200 if thread is successfully added
                if (await postQuery.AddThread(body))
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
            
            return new OkObjectResult(await postQuery.GetThreads());
        }

        //POST /api/posts/add/post
        [HttpPost("add/post")]
        public async Task<IActionResult> PostPost([FromBody] PostModel body, [FromHeader] string token)
        {
            if(!body.IsValidPost())
            {
                return new NotFoundObjectResult("Invalid body");
            }

            await Db.Connection.OpenAsync();

            var tokenModel = await authorizationQuery.GetTokenModel(token);
            //check that user is logged in
            if (tokenModel != null)
            {
                body.user_id = tokenModel.user_id;
                if (await postQuery.AddPost(body))
                {
                    return new OkResult();
                }
                else
                {
                    return new NotFoundObjectResult("Could not add post");
                }
            }

            return new NotFoundObjectResult("User must be logged in to create post");
        }

        //GET /api/posts/posts
        [HttpGet ("posts")]
        public async Task<IActionResult> GetPosts([FromBody] ThreadModel body, [FromHeader] string token)
        {
            if(!body.thread_id.HasValue)
            {
                return new NotFoundObjectResult("Invalid body");
            }
            
            await Db.Connection.OpenAsync();

            //check that user is logged in
            var userToken = await authorizationQuery.GetTokenModel(token);
            var userLocation = await userQuery.GetUserLocation(userToken.user_id);
            if (userToken != null)
            {
                //return list of posts
                return new OkObjectResult(await postQuery.GetPosts(body, userToken, userLocation));
            }

            return new NotFoundObjectResult("User must be logged in to view posts");
        }
    }
}