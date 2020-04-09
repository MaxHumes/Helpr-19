using System;
using System.Collections.Generic;
using System.Data.Common;
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

        //query database to return everything from user_info table
        public async Task<List<UserModel>> UserList()
        {
            using (var cmd = Db.Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM login_info";
                
                //create reader from command and read all rows given
                var reader = await cmd.ExecuteReaderAsync();
                return await readAllAsync(reader);
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
                    int uid = await getUserId(user.email);
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

        //method which determines whether a field is already used in the users database
        public async Task<bool> FieldTaken(string columnName, string value)
        {
            if(!validColumnName(columnName))
            {
                throw new ArgumentException();
            }

            //set database string based on columnName
            string table = String.Equals(columnName, "email") || String.Equals(columnName, "password") ? "login_info" : "personal_info";

            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL command to find rows where value is in columnName
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

        //read user_id from database for user with email = email, returns -1 if no such user is found
        private async Task<int> getUserId(string email)
        {
            using(var cmd = Db.Connection.CreateCommand())
            {
                //create query which returns user with given email
                cmd.CommandText = "SELECT user_id " +
                    "FROM login_info " +
                    "WHERE email = @email";
                cmd.Parameters.AddWithValue("@email", email);

                //read result from query and return user_id if user exists, -1 otherwise
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    if(reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

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
    }
}
