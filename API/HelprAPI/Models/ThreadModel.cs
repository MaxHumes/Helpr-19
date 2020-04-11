using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelprAPI.Models
{
    public class ThreadModel
    {
        public int? thread_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public ThreadModel() { }

        public bool IsValidThread()
        {
            if(name == null || description == null)
            {
                return false;
            }

            return true;
        }
    }
}
