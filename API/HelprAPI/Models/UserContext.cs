using Microsoft.EntityFrameworkCore;

namespace HelprAPI.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) {}

        public DbSet<UserModel> TodoItems { get; set; }
    }
}