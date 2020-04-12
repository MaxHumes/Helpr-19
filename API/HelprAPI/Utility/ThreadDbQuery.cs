using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelprAPI.Utility
{
    public class ThreadDbQuery
    {
        private AppDb Db;
        public ThreadDbQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<bool> ThreadExists(int tid)
        {
            await Db.Connection.ChangeDatabaseAsync("posts");

            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to find rows where user_id is uid
                cmd.CommandText = "SELECT * FROM threads " +
                    "WHERE thread_id = @thread_id";
                cmd.Parameters.AddWithValue("thread_id", tid);

                //if the SQL query returns any rows, return true, false otherwise
                using (var reader = await cmd.ExecuteReaderAsync())
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
    }
}
