using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models;

namespace Roulette.Services;

/// <summary>
///     Сервис для управления историей выбора пользователя.
/// </summary>
/// <remarks>
///     Этот сервис предоставляет методы для добавления записей о выборе игр, аниме, манги и ранобэ пользователями,
///     а также для получения истории выбора с поддержкой постраничного вывода.
/// </remarks>
public class UserChoiceHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserChoiceHistoryService> _logger;

    /// <summary>
    ///     Конструктор сервиса.
    /// </summary>
    /// <param name="context">Контекст базы данных для доступа к информации о выборе пользователя.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public UserChoiceHistoryService(ApplicationDbContext context, ILogger<UserChoiceHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    ///     Добавляет выбор игры в историю пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Задача, представляющая собой асинхронную операцию добавления.</returns>
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

            _logger.LogInformation("Добавлен выбор игры (ID: {GameId}) для пользователя (ID: {UserId}).", gameId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора игры (ID: {GameId}) для пользователя (ID: {UserId}).", gameId, userId);
            throw;
        }
    }

    /// <summary>
    ///     Добавляет выбор аниме в историю пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="animeId">Идентификатор аниме.</param>
    /// <returns>Задача, представляющая собой асинхронную операцию добавления.</returns>
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

            _logger.LogInformation("Добавлен выбор аниме (ID: {AnimeId}) для пользователя (ID: {UserId}).", animeId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора аниме (ID: {AnimeId}) для пользователя (ID: {UserId}).", animeId, userId);
            throw;
        }
    }

    /// <summary>
    ///     Добавляет выбор манги в историю пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="mangaId">Идентификатор манги.</param>
    /// <returns>Задача, представляющая собой асинхронную операцию добавления.</returns>
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

            _logger.LogInformation("Добавлен выбор манги (ID: {MangaId}) для пользователя (ID: {UserId}).", mangaId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора манги (ID: {MangaId}) для пользователя (ID: {UserId}).", mangaId, userId);
            throw;
        }
    }

    /// <summary>
    ///     Добавляет выбор ранобэ в историю пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="ranobeId">Идентификатор ранобэ.</param>
    /// <returns>Задача, представляющая собой асинхронную операцию добавления.</returns>
    public async Task AddRanobeToHistoryAsync(string userId, int ranobeId)
    {
        try
        {
            var userRanobeChoice = new UserRanobeChoice
            {
                UserId = userId,
                RanobeId = ranobeId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserRanobeChoices.Add(userRanobeChoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Добавлен выбор ранобэ (ID: {RanobeId}) для пользователя (ID: {UserId}).", ranobeId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении выбора ранобэ (ID: {RanobeId}) для пользователя (ID: {UserId}).", ranobeId, userId);
            throw;
        }
    }

    /// <summary>
    ///     Получает историю выбора игр пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список записей о выборе игр пользователя.</returns>
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

    /// <summary>
    ///     Получает историю выбора аниме пользователя с поддержкой постраничного вывода.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="pageNumber">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Объект, содержащий список записей о выборе аниме и общее количество записей.</returns>
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

    /// <summary>
    ///     Получает историю выбора манги пользователя с поддержкой постраничного вывода.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="pageNumber">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Объект, содержащий список записей о выборе манги и общее количество записей.</returns>
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

    /// <summary>
    ///     Получает историю выбора ранобэ пользователя с поддержкой постраничного вывода.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="pageNumber">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Объект, содержащий список записей о выборе ранобэ и общее количество записей.</returns>
    public async Task<PaginatedRanobeHistory> GetUserRanobeHistoryAsync(string userId, int pageNumber, int pageSize)
    {
        try
        {
            var totalItems = await _context.UserRanobeChoices
                .Where(urc => urc.UserId == userId)
                .CountAsync();

            var items = await _context.UserRanobeChoices
                .Where(urc => urc.UserId == userId)
                .OrderByDescending(urc => urc.ChosenAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedRanobeHistory
            {
                Items = items,
                TotalCount = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении истории выбора ранобэ для пользователя (ID: {UserId}).", userId);
            throw;
        }
    }
}