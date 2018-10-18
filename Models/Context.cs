using Microsoft.EntityFrameworkCore;
 
namespace weddingplanner.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
	    public DbSet<User> User { get; set; }
        public DbSet<Wedding> Wedding { get; set; }
        public DbSet<Guest> Guest { get; set; }

    }
}
