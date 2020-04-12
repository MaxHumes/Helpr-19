using System.Threading.Tasks;
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
        private ThreadDbQuery threadQuery;
        private LocationQuery locationQuery;
        public PostsController(AppDb db)
        {
            Db = db;
            userQuery = new UserDbQuery(Db);
            postQuery = new PostDbQuery(Db);
            authorizationQuery = new AuthorizationQuery(Db);
            threadQuery = new ThreadDbQuery(Db);
            locationQuery = new LocationQuery(Db);
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
        [HttpGet("get/threads")]
        public async Task<IActionResult> GetThreads()
        {
            await Db.Connection.OpenAsync();
            
            return new OkObjectResult(await postQuery.GetThreads());
        }

        //POST /api/posts/add/post
        [HttpPost("add/post")]
        public async Task<IActionResult> PostPost([FromBody] PostModel body, [FromHeader] string token)
        {
            await Db.Connection.OpenAsync();
            if (!body.IsValidPost() || !await threadQuery.ThreadExists((int)body.thread_id))
            {
                return new NotFoundObjectResult("Invalid body");
            }

            var tokenModel = await authorizationQuery.GetTokenModel(token);
            //check that user is logged in
            if (tokenModel != null)
            {
                //set post's user_id and location if applicable
                body.user_id = tokenModel.user_id;
                var userLocation = await locationQuery.GetLocation((int)body.user_id);
                if(userLocation != null)
                {
                    body.latitude = userLocation.latitude;
                    body.longitude = userLocation.longitude;
                }

                //add post
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
        [HttpGet ("get/{id}")]
        public async Task<IActionResult> GetPosts([FromRoute] int id, [FromHeader] string token)
        {            
            await Db.Connection.OpenAsync();

            //check that user is logged in
            var userToken = await authorizationQuery.GetTokenModel(token);
            var userLocation = await userQuery.GetUserLocation(userToken.user_id);
            if (userToken != null)
            {
                //return list of posts
                return new OkObjectResult(await postQuery.GetPosts(id, userToken, userLocation));
            }

            return new NotFoundObjectResult("User must be logged in to view posts");
        }
    }
}