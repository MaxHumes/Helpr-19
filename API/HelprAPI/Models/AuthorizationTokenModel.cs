using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelprAPI.Models
{
    public class AuthorizationTokenModel
    {
        public int user_id { get; set; }
        public string token { get; set; }

        public AuthorizationTokenModel() { }
    }
}
