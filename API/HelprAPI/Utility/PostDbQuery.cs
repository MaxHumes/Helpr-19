﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using HelprAPI.Models;
using MySql.Data.MySqlClient;

namespace HelprAPI.Utility
{
    public class PostDbQuery
    {
        private AppDb Db;
        public PostDbQuery(AppDb db)
        {
            Db = db;
        }

        //add thread to threads table given name and description
        public async Task<bool> AddThread(ThreadModel thread)
        {
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
            using(var cmd = Db.Connection.CreateCommand())
            {
                //create SQL query and read all thread rows returned back
                cmd.CommandText = "SELECT * FROM threads";
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    return await readAllAsync(reader);
                }
            }
        }

        //read all rows of threads table and return list of threads
        private async Task<List<ThreadModel>> readAllAsync(DbDataReader reader)
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
    }
}
