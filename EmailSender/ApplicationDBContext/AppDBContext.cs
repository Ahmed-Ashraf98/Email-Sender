using EmailSender.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailSender.ApplicationDBContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

    }
}
