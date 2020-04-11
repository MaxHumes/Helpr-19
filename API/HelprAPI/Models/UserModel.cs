namespace HelprAPI.Models
{
    public class UserModel
    {
        public int user_id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public byte[] salt { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string bio { get; set; }

        public UserModel() {}

        public bool IsValidUser()
        {
            if(email == null | password == null | username == null)
            {
                return false;
            }
            return true;
        }

        public bool IsValidLogin()
        {
            if (email == null | password == null)
            {
                return false;
            }
            return true;
        }
    }
}
