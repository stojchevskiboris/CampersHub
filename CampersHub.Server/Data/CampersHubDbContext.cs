using CampersHub.Server.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CampersHub.Server.Data
{
    public class CampersHubDbContext : DbContext
    {
        public CampersHubDbContext(DbContextOptions<CampersHubDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts{ get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Eager loading configuration
            modelBuilder.Entity<Post>()
                .Navigation(p => p.MediaFiles)
                .AutoInclude();

            modelBuilder.Entity<Post>()
                .Navigation(p => p.Tags)
                .AutoInclude();
        }

    }
}
