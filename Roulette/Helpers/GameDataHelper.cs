using Roulette.Data;
using Roulette.Models.Games;

namespace Roulette.Helpers
{
    public class GameDataHelper
    {
        private readonly ApplicationDbContext _context;

        public GameDataHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        //public void UpdateGameGenres(Game game, List<int> genreIds)
        //{
        //    // Удаляем старые жанры
        //    var existingGenres = _context.GameGenres.Where(gg => gg.GameId == game.AppID).ToList();
        //    _context.GameGenres.RemoveRange(existingGenres);

        //    // Добавляем новые жанры
        //    foreach (var genreId in genreIds)
        //    {
        //        _context.GameGenres.Add(new GameGenre { GameId = game.AppID, GenreId = genreId });
        //    }
        //}

        //public void UpdateGameLanguages(Game game, List<int> languageIds)
        //{
        //    // Удаляем старые языки
        //    var existingLanguages = _context.GameSupportedLanguages.Where(gl => gl.GameId == game.AppID).ToList();
        //    _context.GameSupportedLanguages.RemoveRange(existingLanguages);

        //    // Добавляем новые языки
        //    foreach (var languageId in languageIds)
        //    {
        //        _context.GameSupportedLanguages.Add(new GameSupportedLanguage { GameId = game.AppID, LanguageId = languageId });
        //    }
        //}

        //public void AddGameGenres(Game game, List<int> genreIds)
        //{
        //    foreach (var genreId in genreIds)
        //    {
        //        _context.GameGenres.Add(new GameGenre { GameId = game.AppID, GenreId = genreId });
        //    }
        //}

        //public void AddGameLanguages(Game game, List<int> languageIds)
        //{
        //    foreach (var languageId in languageIds)
        //    {
        //        _context.GameSupportedLanguages.Add(new GameSupportedLanguage { GameId = game.AppID, LanguageId = languageId });
        //    }
        //}
    }
}
