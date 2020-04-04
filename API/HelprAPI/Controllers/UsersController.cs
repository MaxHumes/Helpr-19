using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelprAPI.Models;

namespace HelprAPI.Controllers
{    
    [Route("api/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //fields for accessing database connection and querying the datbase
        private AppDb Db;
        private UserDbQuery Query;
        
        public UsersController(AppDb db)
        {
            Db = db;
            Query = new UserDbQuery(Db);
        }

        // GET api/users
        // returns list of all users in user database
        [HttpGet("users")]
        public async Task<IActionResult> GetUserList()
        {
            await Db.Connection.OpenAsync();
            var result = await Query.UserList();
            return new OkObjectResult(result);
        }
        
        //POST api/user
        [HttpPost("user")]
        public async Task<IActionResult> PostUser([FromBody] UserModel body)
        {
            await Db.Connection.OpenAsync();

            //return 200 if user was successfully added, 404 if user already exists
            if (await Query.AddUser(body))
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}