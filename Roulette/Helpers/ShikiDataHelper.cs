using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Helpers;

/// <summary>
/// Хелпер для преобразования данных из Shikimori в модель GenreModel.
/// </summary>
public class ShikiDataHelper
{
    /// <summary>
    /// Преобразует массив жанров из Shikimori в массив GenreModel.
    /// </summary>
    /// <param name="genres">Массив жанров из Shikimori.</param>
    /// <returns>Массив GenreModel.</returns>
    public GenreModel[] TransformGenres(Genre[]? genres)
    {
        return genres
            .Select(g => new
            {
                Id = (int)g.Id,
                Name = g.Name,
                Russian = g.Russian.Replace('ё', 'е'),
                Kind = g.Kind
            })
            .GroupBy(g => new { g.Name, g.Russian, g.Kind })
            .Select(group => new GenreModel
            {
                Ids = group.Select(g => g.Id).ToList(),
                Name = group.Key.Name,
                Russian = group.Key.Russian,
                Kind = group.Key.Kind
            })
            .OrderBy(g => g.Russian)
            .ToArray();
    }
}