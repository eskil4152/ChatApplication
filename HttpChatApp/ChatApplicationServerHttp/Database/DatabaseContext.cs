using ChatApplicationServerHttp;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServerHttp
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the PostgreSQL connection
            optionsBuilder.UseNpgsql("YourConnectionString");
        }
    }
}
