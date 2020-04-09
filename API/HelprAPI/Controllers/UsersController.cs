using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelprAPI.Models;
using HelprAPI.Utility;
using System.Text;

namespace HelprAPI.Controllers
{
    [Route("api/users")]
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

        // GET api/users/list
        // returns list of all users in user database
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList()
        {
            await Db.Connection.OpenAsync();
            var result = await Query.UserList();
            return new OkObjectResult(result);
        }

        //POST api/users/add
        [HttpPost("add")]
        public async Task<IActionResult> PostNewUser([FromBody] UserModel body)
        {
            await Db.Connection.OpenAsync();

            //check that valid email address was inputted and email and username do not match any others in the database
            if (!IsValidEmail(body.email))
            {
                return new NotFoundObjectResult("Invalid email address");
            }
            if(await Query.FieldTaken("email", body.email))
            {
                return new NotFoundObjectResult("Email already taken");
            }
            if(await Query.FieldTaken("username", body.username))
            {
                return new NotFoundObjectResult("Username already taken");
            }

            //generate salt and add to user model
            byte[] salt = Security.GetSalt();
            body.salt = salt;

            //hash password with previously genereated salt and add to user model
            body.password = Security.HashPassword(body.password, salt);

            //return 200 if user was successfully added, 404 if user already exists
            if (await Query.AddUserLogin(body))
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundObjectResult("Server Error");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}