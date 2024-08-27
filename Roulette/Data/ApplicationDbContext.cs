using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Roulette.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().Ignore(u => u.LockoutEnd);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.LockoutEnabled);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.TwoFactorEnabled);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.PhoneNumberConfirmed);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "common_user", NormalizedName = "COMMON_USER" }
            );
        }
    }
}
