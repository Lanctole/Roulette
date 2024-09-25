using Games.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Roulette.DTOs;
using Roulette.Models;
using Genre = Games.Classes.Genre;

namespace Roulette.Data;

/// <summary>
/// Контекст базы данных приложения, управляет доступом к данным и настройками базы данных.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>
    /// Набор данных для игр.
    /// </summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>
    /// Набор данных для жанров.
    /// </summary>
    public DbSet<Genre> Genres { get; set; }

    /// <summary>
    /// Набор данных для поддерживаемых языков.
    /// </summary>
    public DbSet<SupportedLanguage> SupportedLanguages { get; set; }

    /// <summary>
    /// Набор данных для аниме.
    /// </summary>
    public DbSet<AnimeDto> Animes { get; set; }

    /// <summary>
    /// Набор данных для манги.
    /// </summary>
    public DbSet<MangaDto> Mangas { get; set; }

    /// <summary>
    /// Набор данных для ранобэ.
    /// </summary>
    public DbSet<RanobeDto> Ranobes { get; set; }

    /// <summary>
    /// Набор данных для выбора игр пользователями.
    /// </summary>
    public DbSet<UserGameChoice> UserGameChoices { get; set; }

    /// <summary>
    /// Набор данных для выбора аниме пользователями.
    /// </summary>
    public DbSet<UserAnimeChoice> UserAnimeChoices { get; set; }

    /// <summary>
    /// Набор данных для выбора манги пользователями.
    /// </summary>
    public DbSet<UserMangaChoice> UserMangaChoices { get; set; }

    /// <summary>
    /// Набор данных для выбора ранобэ пользователями.
    /// </summary>
    public DbSet<UserRanobeChoice> UserRanobeChoices { get; set; }

    /// <summary>
    /// Набор данных для отчетов о баге.
    /// </summary>
    public DbSet<BugReport> BugReports { get; set; }

    /// <summary>
    /// Набор данных для логов
    /// </summary>
    public DbSet<LogEntry> Logs { get; set; }

    /// <summary>
    /// Конструктор контекста базы данных.
    /// </summary>
    /// <param name="options">Настройки контекста базы данных.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Конфигурация модели данных.
    /// </summary>
    /// <param name="modelBuilder">Модель данных.</param>
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