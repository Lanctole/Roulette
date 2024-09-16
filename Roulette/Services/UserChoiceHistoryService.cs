using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Roulette.Data;
using Roulette.Models;

namespace Roulette.Services
{
    public class UserChoiceHistoryService
    {
        private readonly ApplicationDbContext _context;

        public UserChoiceHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddGameToHistoryAsync(string userId, int gameId)
        {
            var userGameChoice = new UserGameChoice
            {
                UserId = userId,
                GameId = gameId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserGameChoices.Add(userGameChoice);
            await _context.SaveChangesAsync();
        }

        public async Task AddAnimeToHistoryAsync(string userId, int animeId)
        {
            var userAnimeChoice = new UserAnimeChoice
            {
                UserId = userId,
                AnimeId = animeId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserAnimeChoices.Add(userAnimeChoice);
            await _context.SaveChangesAsync();
        }

        public async Task AddMangaToHistoryAsync(string userId, int mangaId)
        {
            var userMangaChoice = new UserMangaChoice
            {
                UserId = userId,
                MangaId = mangaId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserMangaChoices.Add(userMangaChoice);
            await _context.SaveChangesAsync();
        }

        public async Task AddRanobeToHistoryAsync(string userId, int ranobeId)
        {
            var userRanobeChoice = new UserRanobeChoice
            {
                UserId = userId,
                RanobeId = ranobeId,
                ChosenAt = DateTime.UtcNow
            };

            _context.UserRanobeChoices.Add(userRanobeChoice);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserGameChoice>> GetUserGameHistoryAsync(string userId)
        {
            return await _context.UserGameChoices
                .Where(ugc => ugc.UserId == userId)
                .OrderByDescending(ugc => ugc.ChosenAt)
                .ToListAsync();
        }

        public async Task<PaginatedGameHistory> GetUserGameHistoryAsync(string userId, int pageNumber, int pageSize)
        {
            var totalItems = await _context.UserGameChoices
                .Where(ugc => ugc.UserId == userId)
                .CountAsync();

            var items = await _context.UserGameChoices
                .Where(ugc => ugc.UserId == userId)
                .OrderByDescending(ugc => ugc.ChosenAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedGameHistory
            {
                Items = items,
                TotalCount = totalItems
            };
        }

        public async Task<List<UserAnimeChoice>> GetUserAnimeHistoryAsync(string userId)
        {
            return await _context.UserAnimeChoices
                .Where(uac => uac.UserId == userId)
                .OrderByDescending(uac => uac.ChosenAt)
                .ToListAsync();
        }

        public async Task<PaginatedAnimeHistory> GetUserAnimeHistoryAsync(string userId, int pageNumber, int pageSize)
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

        public async Task<List<UserMangaChoice>> GetUserMangaHistoryAsync(string userId)
        {
            return await _context.UserMangaChoices
                .Where(umc => umc.UserId == userId)
                .OrderByDescending(umc => umc.ChosenAt)
                .ToListAsync();
        }

        public async Task<PaginatedMangaHistory> GetUserMangaHistoryAsync(string userId, int pageNumber, int pageSize)
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

        public async Task<List<UserRanobeChoice>> GetUserRanobeHistoryAsync(string userId)
        {
            return await _context.UserRanobeChoices
                .Where(urc => urc.UserId == userId)
                .OrderByDescending(urc => urc.ChosenAt)
                .ToListAsync();
        }

        public async Task<PaginatedRanobeHistory> GetUserRanobeHistoryAsync(string userId, int pageNumber, int pageSize)
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
    }
}
