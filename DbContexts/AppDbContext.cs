using Microsoft.EntityFrameworkCore;
using OutboxPattern.Entities;

namespace OutboxPattern.DbContexts
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
