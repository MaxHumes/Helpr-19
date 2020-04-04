namespace HelprAPI.Models
{
    public class UserModel
    {
        public int user_id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }

        public UserModel() {}
    }
}
