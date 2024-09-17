using Games.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Roulette.DTOs;
using Roulette.Models;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using Genre = Games.Classes.Genre;

namespace Roulette.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<SupportedLanguage> SupportedLanguages{ get; set; }
        public DbSet<AnimeDto> Animes { get; set; } 
        public DbSet<MangaDto> Mangas { get; set; }
        public DbSet<RanobeDto> Ranobes { get; set; }
        public DbSet<UserGameChoice> UserGameChoices { get; set; }
        public DbSet<UserAnimeChoice> UserAnimeChoices { get; set; }
        public DbSet<UserMangaChoice> UserMangaChoices { get; set; }
        public DbSet<UserRanobeChoice> UserRanobeChoices { get; set; }
        public DbSet<BugReport> BugReports { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimeDto>()
                .ToTable("Animes")
                .Property(a => a.Content)
                .HasColumnType("jsonb");

            modelBuilder.Entity<MangaDto>()
                .ToTable("Mangas")
                .Property(a => a.Content)
                .HasColumnType("jsonb");

            modelBuilder.Entity<RanobeDto>()
                .ToTable("Ranobes")
                .Property(a => a.Content)
                .HasColumnType("jsonb");

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
