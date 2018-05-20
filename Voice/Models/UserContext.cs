using System.Data.Entity;

namespace Voice.Models
{
    public class UserContext : DbContext
    {
        public UserContext() :
            base("DiplomaConnection")
        { }
        public DbSet<User> Users { get; set; }
    }
}