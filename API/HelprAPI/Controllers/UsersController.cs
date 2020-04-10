using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelprAPI.Models;
using HelprAPI.Utility;
using RandomStringUtils;

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

        //API Calls:

        //POST api/users/add
        [HttpPost("add")]
        public async Task<IActionResult> PostNewUser([FromBody] UserModel body)
        {
            await Db.Connection.OpenAsync();
            await Db.Connection.ChangeDataBaseAsync("users");

            //check that valid email address was inputted and email and username do not match any others in the database
            if (!IsValidEmail(body.email))
            {
                return new NotFoundObjectResult("Invalid email address");
            }
            if(await Query.FieldAlreadyExists("email", body.email))
            {
                return new NotFoundObjectResult("Email already taken");
            }
            if(await Query.FieldAlreadyExists("username", body.username))
            {
                return new NotFoundObjectResult("Username already taken");
            }

            body.email = (body.email).ToLower();

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

        //POST/api/users/login
        [HttpPost ("login")]
        public async Task<IActionResult> PostLogin([FromBody] UserModel body)
        {
            await Db.Connection.OpenAsync();
            await Db.Connection.ChangeDataBaseAsync("users");

            //get user from database with given email
            UserModel user = await Query.GetUser(body.email.ToLower());

            //return 404 code if user is already logged in
            if(await Query.UserLoggedIn(user.user_id))
            {
                return new NotFoundObjectResult("User already logged in");
            }

            //return 404 code if user is not found
            if (user == null)
            {
                return new NotFoundObjectResult("Email not found in database");
            }

            //return 404 code if password is incorrect
            string password = Security.HashPassword(body.password, user.salt);
            if(!await Query.FieldAlreadyExists(("password"), password))
            {
                return new NotFoundObjectResult("Password is incorrect");
            }

            //create login token and send the result to authorization_tokens in database
            string token = user.user_id.ToString() + RandomStringUtils.RandomStringUtils.RandomAlphanumeric(32);
            if(await Query.AddAuthorizationToken(user.user_id, token))
            {
                return new OkObjectResult(token);
            }

            return new NotFoundObjectResult("Error logging in user");
        }

        //POST /api/users/logout
        [HttpPost ("logout")]
        public async Task<IActionResult> PostLogout([FromBody] AuthorizationTokenModel token)
        {
            await Db.Connection.OpenAsync();
            await Db.Connection.ChangeDataBaseAsync("users");

            //return 200 if user is successfully logged out
            if (await Query.Logout(token.token))
            {
                return new OkResult();
            }
            
            return new NotFoundObjectResult("Unable to log user out");
        }

        //Private helper methods:

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