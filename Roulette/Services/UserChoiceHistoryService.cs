using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models;

namespace Roulette.Services;

public class UserChoiceHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserChoiceHistoryService> _logger;

    public UserChoiceHistoryService(ApplicationDbContext context, ILogger<UserChoiceHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddGameToHistoryAsync(string userId, int gameId)
    {
        try
        {
            var userGameChoice = new UserGameChoice
            {
                UserId = userId,
                GameId = gameId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserGameChoices.Add(userGameChoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Добавлен выбор игры (ID: {GameId}) для пользователя (ID: {UserId}).", gameId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора игры (ID: {GameId}) для пользователя (ID: {UserId}).",
                gameId, userId);
            throw;
        }
    }

    public async Task AddAnimeToHistoryAsync(string userId, int animeId)
    {
        try
        {
            var userAnimeChoice = new UserAnimeChoice
            {
                UserId = userId,
                AnimeId = animeId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserAnimeChoices.Add(userAnimeChoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Добавлен выбор аниме (ID: {AnimeId}) для пользователя (ID: {UserId}).", animeId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора аниме (ID: {AnimeId}) для пользователя (ID: {UserId}).",
                animeId, userId);
            throw;
        }
    }

    public async Task AddMangaToHistoryAsync(string userId, int mangaId)
    {
        try
        {
            var userMangaChoice = new UserMangaChoice
            {
                UserId = userId,
                MangaId = mangaId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserMangaChoices.Add(userMangaChoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Добавлен выбор манги (ID: {MangaId}) для пользователя (ID: {UserId}).", mangaId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора манги (ID: {MangaId}) для пользователя (ID: {UserId}).",
                mangaId, userId);
            throw;
        }
    }

    public async Task<List<UserGameChoice>> GetUserGameHistoryAsync(string userId)
    {
        try
        {
            return await _context.UserGameChoices
                .Where(ugc => ugc.UserId == userId)
                .OrderByDescending(ugc => ugc.ChosenAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении истории выбора игр для пользователя (ID: {UserId}).", userId);
            throw;
        }
    }

    public async Task<PaginatedAnimeHistory> GetUserAnimeHistoryAsync(string userId, int pageNumber, int pageSize)
    {
        try
        {
            var totalItems = await _context.UserAnimeChoices
                .Where(uac => uac.UserId == userId)
                .CountAsync();

            var items = await _context.UserAnimeChoices
                .Where(uac => uac.UserId == userId)
                .OrderByDescending(uac => uac.ChosenAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedAnimeHistory
            {
                Items = items,
                TotalCount = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении истории выбора аниме для пользователя (ID: {UserId}).", userId);
            throw;
        }
    }

    public async Task<PaginatedMangaHistory> GetUserMangaHistoryAsync(string userId, int pageNumber, int pageSize)
    {
        try
        {
            var totalItems = await _context.UserMangaChoices
                .Where(umc => umc.UserId == userId)
                .CountAsync();

            var items = await _context.UserMangaChoices
                .Where(umc => umc.UserId == userId)
                .OrderByDescending(umc => umc.ChosenAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedMangaHistory
            {
                Items = items,
                TotalCount = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении истории выбора манги для пользователя (ID: {UserId}).", userId);
            throw;
        }
    }
}