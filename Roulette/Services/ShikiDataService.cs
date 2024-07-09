using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Services
{
    public class ShikiDataService
    {
        public GenreModel[] TransformGenres(Genre[] genres)
        {
            return genres
                .Select(g => new Genre
                {
                    Id = (int)g.Id,
                    Name = g.Name,
                    Russian = g.Russian.Replace('ё', 'е'),
                    Kind = g.Kind
                })
                .GroupBy(g => new { g.Name, g.Russian, g.Kind })
                .Select(group => new GenreModel
                {
                    Ids = group.Select(g => (int)g.Id).ToList(),
                    Name = group.Key.Name,
                    Russian = group.Key.Russian,
                    Kind = group.Key.Kind
                })
                .OrderBy(g => g.Russian)
                .ToArray();
        }
    }
}
