using System.Threading.Tasks;
using HelprAPI.Models;

namespace HelprAPI.Utility
{
    public class LocationQuery
    {
        private AppDb Db;
        public LocationQuery(AppDb db)
        {
            Db = db;
        }
        //query methods:

        //get UserModel from location table where uid = uid
        public async Task<UserModel> GetLocation(int uid)
        {
            await Db.Connection.ChangeDataBaseAsync("users");

            using (var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to find rows where user_id is uid
                cmd.CommandText = "SELECT * FROM location_info WHERE user_id = @user_id";
                cmd.Parameters.AddWithValue("@user_id", uid);

                //if the SQL query returns any rows, return true, false otherwise
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var userModel = new UserModel()
                        {
                            user_id = reader.GetInt32(0)
                        };
                        try
                        {
                            //try to add latitude and longitude to user
                            userModel.latitude = (double?)reader["latitude"];
                            userModel.longitude = (double?)reader["longitude"];

                            return userModel;
                        }
                        catch 
                        {
                            return null;
                        }
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
