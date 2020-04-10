using Microsoft.EntityFrameworkCore;

namespace HelprAPI.Models
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options) {}

        public DbSet<UserModel> Users { get; set; }
    }
}