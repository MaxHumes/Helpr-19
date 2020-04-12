using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using HelprAPI.Models;
using HelprAPI.Utility.PostComparers;

namespace HelprAPI.Utility
{
    public class PostDbQuery
    {
        private AppDb Db;
        private UserDbQuery userQuery;
        public PostDbQuery(AppDb db)
        {
            Db = db;
            userQuery = new UserDbQuery(Db);
        }

        //add thread to threads table given name and description
        public async Task<bool> AddThread(ThreadModel thread)
        {
            await Db.Connection.ChangeDataBaseAsync("posts");

            using (var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query to insert thread into threads table
                cmd.CommandText = "INSERT INTO threads (name, description)" +
                    "VALUES (@name, @description)";
                cmd.Parameters.AddWithValue("@name", thread.name);
                cmd.Parameters.AddWithValue("@description", thread.description);

                //try to execute query, return true if it works, false otherwise
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }

        //get all threads in the threads database
        public async Task<List<ThreadModel>> GetThreads()
        {
            await Db.Connection.ChangeDataBaseAsync("posts");

            using (var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query and read all thread rows returned back
                cmd.CommandText = "SELECT * FROM threads";
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    return await readAllThreadsAsync(reader);
                }
            }
        }

        //add post to posts table given thread_id, user_id, name, and description
        public async Task<bool> AddPost(PostModel post)
        {
            await Db.Connection.ChangeDataBaseAsync("posts");

            using (var cmd = Db.Connection.CreateCommand())
            {
                if(post.GetGeoCoordinates() == null)
                {
                    //create SQL query to insert post into posts table without location
                    cmd.CommandText = "INSERT INTO posts (thread_id, user_id, name, description)" +
                        "VALUES (@thread_id, @user_id, @name, @description)";
                }
                else
                {
                    //create SQL query to insert post into posts table with location
                    cmd.CommandText = "INSERT INTO posts (thread_id, user_id, name, description, latitude, longitude)" +
                        "VALUES (@thread_id, @user_id, @name, @description, @latitude, @longitude)";
                    cmd.Parameters.AddWithValue("@latitude", post.latitude);
                    cmd.Parameters.AddWithValue("@longitude", post.longitude);
                }
                cmd.Parameters.AddWithValue("@thread_id", post.thread_id);
                cmd.Parameters.AddWithValue("@user_id", post.user_id);
                cmd.Parameters.AddWithValue("@name", post.name);
                cmd.Parameters.AddWithValue("@description", post.description);

                //try to execute query, return true if it works, false otherwise
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        //get all posts in given thread
        public async Task<List<PostModel>> GetPosts(ThreadModel thread, AuthorizationTokenModel token, UserModel location)
        {
            await Db.Connection.ChangeDataBaseAsync("posts");
            
            using(var cmd = Db.Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM posts " +
                    "WHERE thread_id = @thread_id";
                cmd.Parameters.AddWithValue("@thread_id", thread.thread_id);
                
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    return await readAllPostsAsync(reader, location);
                }
            }
        }

        //private helper methods:

        //read all rows of threads table and return list of threads
        private async Task<List<ThreadModel>> readAllThreadsAsync(DbDataReader reader)
        {
            var threads = new List<ThreadModel>();
            using (reader)
            {
                //while the reader still has unread rows
                while (await reader.ReadAsync())
                {
                    //add thread to list
                    var thread = new ThreadModel()
                    {
                        thread_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        description = reader.GetString(2)
                    };
                    threads.Add(thread);
                }
            }
            return threads;
        }

        //read all rows of posts and return list of posts sorted by distance
        private async Task<List<PostModel>> readAllPostsAsync(DbDataReader reader, UserModel location)
        {
            var posts = new List<PostModel>();
            var userCoordinates = location.GetGeoCoordinates();
            using (reader)
            {
                //while the reader still has unread rows
                while (await reader.ReadAsync())
                {
                    //read post from response
                    var post = new PostModel()
                    {
                        post_id = reader.GetInt32(0),
                        thread_id = reader.GetInt32(1),
                        user_id = reader.GetInt32(2),
                        name = reader.GetString(3),
                        description = reader.GetString(4),
                    };
                    //if user and post geocoordinates are not null, set the post's distance from user
                    try
                    {
                        post.latitude = (double?)reader["latitude"];
                        post.longitude = (double?)reader["longitude"];

                        post.SetDistFromUser(userCoordinates);
                    }
                    catch { }
                    posts.Add(post);
                }
            }

            //sort posts in order of ascending distance from the user
            posts.Sort(new DistanceComparer().Compare);
            return posts;
        }
    }
}
