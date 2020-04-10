using System;
using System.Threading.Tasks;
using HelprAPI.Models;
using MySql.Data.MySqlClient;

namespace HelprAPI.Utility
{
    public class AuthorizationQuery
    {
        private AppDb Db;
        public AuthorizationQuery(AppDb db)
        {
            Db = db;
        }

        //query methods:
        
        //get TokeModel from database where token = token
        public async Task<AuthorizationTokenModel> GetTokenModel(string token)
        {
            await Db.Connection.ChangeDataBaseAsync("users");

            using (var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to find rows where user_id is uid
                cmd.CommandText = "SELECT * FROM authorization_tokens WHERE token = @token";
                cmd.Parameters.AddWithValue("@token", token);

                //if the SQL query returns any rows, return true, false otherwise
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var tokenModel = new AuthorizationTokenModel()
                        {
                            user_id = reader.GetInt32(0),
                            token = reader.GetString(1)
                        };
                        return tokenModel;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
