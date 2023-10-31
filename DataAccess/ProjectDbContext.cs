using Microsoft.EntityFrameworkCore;
using UserAuthAPI.DataAccess.Configurations;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Group> Groups { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OTPConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
