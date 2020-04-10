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
        private PostDbQuery Query;
        public PostsController(AppDb db)
        {
            Db = db;
            Query = new PostDbQuery(Db);
        }


        //API Calls:

        //POST /api/posts/add/thread
        [HttpPost ("add/thread")]
        public async Task<IActionResult> PostNewThread([FromBody] ThreadModel body)
        {
            await Db.Connection.OpenAsync();
            await Db.Connection.ChangeDataBaseAsync("posts");

            //return 200 if thread is successfully added
            if (await Query.AddThread(body))
            {
                return new OkResult();
            }

            return new NotFoundObjectResult("Could not add thread");
        }

        //GET /api/posts/threads
        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads()
        {
            await Db.Connection.OpenAsync();
            await Db.Connection.ChangeDataBaseAsync("posts");
            
            return new OkObjectResult(await Query.GetThreads());
        }
    }
}