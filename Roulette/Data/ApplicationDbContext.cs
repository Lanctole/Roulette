using Games.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Roulette.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<SupportedLanguage> SupportedLanguages{ get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.LockoutEnd);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.LockoutEnabled);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.TwoFactorEnabled);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<IdentityUser>().Ignore(u => u.PhoneNumberConfirmed);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "af39bec8-183b-44eb-bf0e-4dec9ddad541", Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "081c2bbb-69dc-4e29-9de0-0d72e1839227", Name = "common_user", NormalizedName = "COMMON_USER" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
