using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using HelprAPI.Models;

namespace HelprAPI
{
    public class UserDbQuery
    {
        AppDb Db { get; }

        public UserDbQuery(AppDb db)
        {
            Db = db;
        }

        //query database to return everything from user_info table
        public async Task<List<UserModel>> UserList()
        {
            using (var cmd = Db.Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM user_info";
                
                //create reader from command and read all rows given
                var reader = await cmd.ExecuteReaderAsync();
                return await ReadAllAsync(reader);
            }
        }

        public async Task<bool> AddUser(UserModel user)
        {
            using (var cmd = Db.Connection.CreateCommand())
            {
                cmd.CommandText = $"INSERT INTO user_info (email, password, username, full_name)" +
                                  $"VALUES ('{user.email}', '{user.password}', '{user.username}', '{user.full_name}')";
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        //read all rows of user_info and return list of UserModels
        private async Task<List<UserModel>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<UserModel>();
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
                        username = reader.GetString(3),
                        full_name = reader.GetString(4)
                    };
                    posts.Add(user);
                }
            }
            return posts;
        }
    }
}
