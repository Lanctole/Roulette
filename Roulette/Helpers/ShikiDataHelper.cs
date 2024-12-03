using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Helpers;

/// <summary>
/// Хелпер для преобразования данных из Shikimori в модель GenreModel.
/// </summary>
public class ShikiDataHelper
{
    /// <summary>
    /// Преобразует массив жанров из Shikimori в упорядоченный массив GenreModel.
    /// </summary>
    /// <param name="genres">Массив жанров.</param>
    /// <returns>Упорядоченный массив GenreModel.</returns>
    public static GenreModel[] TransformGenres(IEnumerable<GenreModel>? genres)
    {
        return genres?
            .Where(g => g != null)
            .OrderBy(g => g.Russian)
            .ToArray() ?? Array.Empty<GenreModel>();
    }
}