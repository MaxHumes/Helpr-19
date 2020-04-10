using System;
using System.Threading.Tasks;
using HelprAPI.Models;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace HelprAPI
{
    public class UserDbQuery
    {
        private AppDb Db;

        public UserDbQuery(AppDb db)
        {
            Db = db;
        }

        //query methods:

        //get user as UserModel from database where email = email
        public async Task<UserModel> GetUser(string email)
        {
            using (var cmd = Db.Connection.CreateCommand())
            {
                UserModel user = new UserModel();

                //create query which returns user login info with given email
                cmd.CommandText = "SELECT * " +
                    "FROM login_info " +
                    "WHERE email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                //read result from login_info query set user login info
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        user.user_id = reader.GetInt32(0);
                        user.email = email;
                        user.password = reader.GetString(2);
                        user.salt = (byte[])reader["salt"];
                    }
                    else
                    {
                        return null;
                    }
                }

                //create a query which returns user personal info with given email
                cmd.CommandText = "SELECT * " +
                    "FROM personal_info " +
                    "WHERE user_id = @user_id";
                cmd.Parameters.AddWithValue("@user_id", user.user_id);

                //read result from personal_info query and set personal info
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        user.username = reader.GetString(1);
                        user.full_name = reader.GetString(2);
                        user.bio = reader.GetString(3);
                    }
                    else
                    {
                        return null;
                    }
                }

                return user;
            }
        }

        //add user info given model to both login_info and personal_info
        public async Task<bool> AddUserLogin(UserModel user)
        {
            //connect to database
            using (var cmd = Db.Connection.CreateCommand())
            {
                //return true if there are no errors in adding user to database, false otherwise
                try
                {
                    //add text for login_info SQL query
                    cmd.CommandText = "INSERT INTO login_info (email, password, salt)" +
                                      "VALUES (@email, @password, @salt)";
                    //add email, password, and salt as parameters
                    cmd.Parameters.AddWithValue("@email", user.email);
                    cmd.Parameters.AddWithValue("@password", user.password);
                    var saltParam = new MySqlParameter("@salt", MySqlDbType.Blob);
                    saltParam.Value = user.salt;
                    cmd.Parameters.Add(saltParam);
                    //execute command
                    await cmd.ExecuteNonQueryAsync();

                    //add text for personal_info SQL query
                    cmd.CommandText = "INSERT INTO personal_info (user_id, username, full_name, bio)" +
                        "VALUES (@user_id, @username, @full_name, @bio)";
                    //add user_id, username, full_name, and bio as parameters
                    //check that we get a valid uid before adding as parameter
                    int uid = (await GetUser(user.email)).user_id;
                    if(uid < 0)
                    {
                        return false;
                    }
                    cmd.Parameters.AddWithValue("@user_id", uid);
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@full_name", user.full_name);
                    cmd.Parameters.AddWithValue("@bio", user.bio);
                    //execute command
                    await cmd.ExecuteNonQueryAsync();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        //add user token to authorization_tokens table
        public async Task<bool> AddAuthorizationToken(int uid, string token)
        {
            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to insert token as active token
                cmd.CommandText = "INSERT INTO authorization_tokens (user_id, token)" +
                    "VALUES (@user_id, @token)";
                cmd.Parameters.AddWithValue("@user_id", uid);
                cmd.Parameters.AddWithValue("@token", token);

                //return true if token is successfully added
                if(await cmd.ExecuteNonQueryAsync() > 0)
                {
                    return true;
                }

                return false;
            }
        }

        //logout user using their authorization token
        public async Task<bool> Logout(string token)
        {
            using(var cmd = Db.Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM authorization_tokens " +
                    "WHERE token = @token";
                cmd.Parameters.AddWithValue("@token", token);

                //if the SQL query returns any rows, delete user and return true, return false otherwise
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        await reader.CloseAsync();
                        
                        //attempt to delete token and return true if token is deleted
                        cmd.CommandText = "DELETE FROM authorization_tokens WHERE token = @token";
                        if(await cmd.ExecuteNonQueryAsync() > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        //method which determines whether a field is already used in the users database
        public async Task<bool> FieldAlreadyExists(string columnName, string value)
        {
            if(!validColumnName(columnName))
            {
                throw new ArgumentException();
            }

            //set database string based on columnName
            string table = String.Equals(columnName, "email") || String.Equals(columnName, "password") ? "login_info" : "personal_info";

            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to find rows where value is in columnName
                cmd.CommandText = $"SELECT * FROM {table} WHERE {columnName} = @value";
                cmd.Parameters.AddWithValue("@value", value);

                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    //if select statement read any users, return that field is taken
                    if(reader.Read())
                    {
                        return true;
                    }
                }
            }

            //otherwise field is not taken
            return false;
        }

        //method which determines whether a user is logged in by user_id
        public async Task<bool> UserLoggedIn(int uid)
        {
            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to find rows where user_id is uid
                cmd.CommandText = "SELECT * FROM authorization_tokens WHERE user_id = @user_id";
                cmd.Parameters.AddWithValue("@user_id", uid);

                //if the SQL query returns any rows, return true, false otherwise
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    if(reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        //Private helper methods:

        //determine whether col is a valid column name for SQL query
        private bool validColumnName(string col)
        {
            //get properties from UserModel
            PropertyInfo[] properties = typeof(UserModel).GetProperties();
            foreach(PropertyInfo p in properties)
            {
                //check whether col equals property name
                if(String.Equals(col, p.Name))
                {
                    return true;
                }
            }
            return false;
        }

        /*
        //read all rows of user_info and return list of UserModels
        private async Task<List<UserModel>> readAllAsync(DbDataReader reader)
        {
            var users = new List<UserModel>();
            using (reader)
            {
                //while the reader still has unread rows
                while (await reader.ReadAsync())
                {
                    //add user to list
                    var user = new UserModel()
                    {
                        user_id = reader.GetInt32(0),
                        email = reader.GetString(1),
                        password = reader.GetString(2),
                        salt = (byte[])reader["salt"]
                    };

                    users.Add(user);
                }
            }
            return users;
        }
        */
    }
}
