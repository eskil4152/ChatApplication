using ChatApplicationServerHttp;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServerHttp
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Room> rooms { get; set; }
        public DbSet<RoomUser> roomuser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=eskil;Username=postgres;Password=password");
        }
    }
}
