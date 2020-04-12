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
        private UserDbQuery userQuery;
        private AuthorizationQuery authorizationQuery;
        public UsersController(AppDb db)
        {
            Db = db;
            userQuery = new UserDbQuery(Db);
            authorizationQuery = new AuthorizationQuery(Db);
        }

        //API Calls:

        //POST api/users/add
        [HttpPost("add")]
        public async Task<IActionResult> PostNewUser([FromBody] UserModel body)
        {
            if(!body.IsValidUser())
            {
                return new NotFoundObjectResult("Invalid body");
            }

            await Db.Connection.OpenAsync();

            //check that valid email address was inputted and email and username do not match any others in the database
            if (!isValidEmail(body.email))
            {
                return new NotFoundObjectResult("Invalid email address");
            }
            if(await userQuery.FieldAlreadyExists("email", body.email))
            {
                return new NotFoundObjectResult("Email already taken");
            }
            if(await userQuery.FieldAlreadyExists("username", body.username))
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
            if (await userQuery.AddUser(body))
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
            if (!body.IsValidLogin())
            {
                return new NotFoundObjectResult("Invalid body");
            }
            
            await Db.Connection.OpenAsync();

            //get user from database with given email
            UserModel user = await userQuery.GetUserLogin(body.email.ToLower());

            //return 404 code if user is already logged in
            if(await userQuery.UserLoggedIn(user.user_id))
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
            if(!await userQuery.FieldAlreadyExists(("password"), password))
            {
                return new NotFoundObjectResult("Password is incorrect");
            }

            //create login token and send the result to authorization_tokens in database
            string token = user.user_id.ToString() + RandomStringUtils.RandomStringUtils.RandomAlphanumeric(32);
            if(await userQuery.AddAuthorizationToken(user.user_id, token))
            {
                return new OkObjectResult(token);
            }

            return new NotFoundObjectResult("Error logging in user");
        }

        //POST /api/users/logout
        [HttpPost ("logout")]
        public async Task<IActionResult> PostLogout([FromHeader] string token)
        {
            await Db.Connection.OpenAsync();

            //return 200 if user is successfully logged out
            if (await userQuery.Logout(token))
            {
                return new OkResult();
            }
            
            return new NotFoundObjectResult("Unable to log user out");
        }

        //POST api/users/location
        [HttpPost ("location")]
        public async Task<IActionResult> PostLocation([FromBody] UserModel body, [FromHeader] string token)
        {
            await Db.Connection.OpenAsync();
            
            //return 404 code if latitude and longitude are not given
            if(body.latitude == null || body.longitude == null)
            {
                return new NotFoundObjectResult("Invalid body");
            }
            if(body.GetGeoCoordinates() == null)
            {
                return new NotFoundObjectResult("Invalid coordinates");
            }

            var userToken = await authorizationQuery.GetTokenModel(token);
            if(userToken != null)
            {
                body.user_id = userToken.user_id;
                if(await userQuery.AddUserLocation(body))
                {
                    return new OkResult();
                }
                {
                    return new NotFoundObjectResult("Server error");
                }
            }

            return new NotFoundObjectResult("User must be logged in to set location");
        }

        //Private helper methods:

        private bool isValidEmail(string email)
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