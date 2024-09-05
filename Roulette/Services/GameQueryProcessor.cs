using Games.Classes;
using Games.Enums;
using Microsoft.EntityFrameworkCore;

namespace Roulette.Services;

public class GameQueryProcessor
{
    public IQueryable<Game> ApplyFilters(
        IQueryable<Game> query,
        string? genres,
        string? supportedLanguages,
        int? metacriticScore,
        int? steamScore,
        double? minCost,
        double? maxCost,
        string? releaseDateStart,
        string? releaseDateEnd)
    {
        if (!string.IsNullOrWhiteSpace(genres))
        {
            var genreIds = genres.Split(',').Select(g => int.Parse(g.Trim())).ToList();
            query = query.Where(g => g.Genres.Select(genre => genre.Id).Intersect(genreIds).Count() == genreIds.Count);
        }

        if (!string.IsNullOrWhiteSpace(supportedLanguages))
        {
            var languageIds = supportedLanguages.Split(',').Select(l => int.Parse(l.Trim())).ToList();
            query = query.Where(g =>
                g.SupportedLanguages.Select(lang => lang.Id).Intersect(languageIds).Count() == languageIds.Count);
        }

        if (metacriticScore.HasValue) query = query.Where(g => g.MetacriticScore >= metacriticScore.Value);

        if (steamScore.HasValue) query = query.Where(g => g.SteamScore >= steamScore.Value);

        if (minCost.HasValue) query = query.Where(g => g.Cost >= minCost.Value);

        if (maxCost.HasValue) query = query.Where(g => g.Cost <= maxCost.Value);

        if (DateTime.TryParse(releaseDateStart, out var startDate))
        {
            startDate = startDate.ToUniversalTime();
            query = query.Where(g => g.ReleaseDate >= startDate);
        }

        if (DateTime.TryParse(releaseDateEnd, out var endDate))
        {
            endDate = endDate.ToUniversalTime();
            query = query.Where(g => g.ReleaseDate <= endDate);
        }

        return query;
    }

    public IQueryable<Game> ApplySorting(
        IQueryable<Game> query,
        GameOrder? order)
    {
        switch (order)
        {
            case GameOrder.Id:
                query = query.OrderBy(g => g.AppID);
                break;
            case GameOrder.SteamScore:
                query = query.OrderByDescending(g => g.SteamScore);
                break;
            case GameOrder.Name:
                query = query.OrderBy(g => g.Name);
                break;
            case GameOrder.ReleaseDate:
                query = query.OrderBy(g => g.ReleaseDate);
                break;
            case GameOrder.Random:
                query = query.OrderBy(g => EF.Functions.Random());
                break;
            default:
                query = query.OrderBy(g => g.Name);
                break;
        }

        return query;
    }
}