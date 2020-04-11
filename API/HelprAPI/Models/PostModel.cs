﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelprAPI.Models
{
    public class PostModel
    {
        public int post_id { get; set; }
        public int? thread_id { get; set; }
        public int? user_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public PostModel() { }

        public bool IsValidPost()
        {
            if (!thread_id.HasValue || !user_id.HasValue || name == null || description == null)
            {
                return false;
            }

            return true;
        }
    }
}