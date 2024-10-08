﻿using Games.Classes;
using Games.Enums;
using Microsoft.EntityFrameworkCore;

namespace Roulette.Services;

/// <summary>
///     Обрабатывает запросы к играм, применяя фильтры и сортировку.
/// </summary>
public class GameQueryProcessor
{
    /// <summary>
    ///     Применяет фильтры к запросу играм.
    /// </summary>
    /// <param name="query">Запрос для фильтрации.</param>
    /// <param name="genres">Строка с идентификаторами жанров, разделёнными запятыми.</param>
    /// <param name="supportedLanguages">Строка с идентификаторами поддерживаемых языков, разделёнными запятыми.</param>
    /// <param name="metacriticScoreMin">Минимальное значение оценки Metacritic.</param>
    /// <param name="metacriticScoreMax">Максимальное значение оценки Metacritic.</param>
    /// <param name="steamScoreMin">Минимальное значение оценки Steam.</param>
    /// <param name="steamScoreMax">Максимальное значение оценки Steam.</param>
    /// <param name="minCost">Минимальная стоимость игры.</param>
    /// <param name="maxCost">Максимальная стоимость игры.</param>
    /// <param name="releaseDateStart">Дата начала диапазона выпуска игр.</param>
    /// <param name="releaseDateEnd">Дата окончания диапазона выпуска игр.</param>
    /// <returns>Отфильтрованный запрос игр.</returns>
    public IQueryable<Game> ApplyFilters(
        IQueryable<Game> query,
        string? genres,
        string? supportedLanguages,
        int? metacriticScoreMin,
        int? metacriticScoreMax,
        int? steamScoreMin,
        int? steamScoreMax,
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

        if (metacriticScoreMin.HasValue || metacriticScoreMax.HasValue)
        {
            if (metacriticScoreMin.HasValue)
                query = query.Where(g => g.MetacriticScore >= metacriticScoreMin.Value);
            if (metacriticScoreMax.HasValue)
                query = query.Where(g => g.MetacriticScore <= metacriticScoreMax.Value);
        }

        if (steamScoreMin.HasValue || steamScoreMax.HasValue)
        {
            if (steamScoreMin.HasValue)
                query = query.Where(g => g.SteamScore >= steamScoreMin.Value);
            if (steamScoreMax.HasValue)
                query = query.Where(g => g.SteamScore <= steamScoreMax.Value);
        }

        if (minCost.HasValue && maxCost.HasValue && minCost == maxCost)
        {
            query = query.Where(g => g.Cost >= minCost.Value - 1 && g.Cost <= maxCost.Value);
        }
        else
        {
            if (minCost.HasValue)
                query = query.Where(g => g.Cost >= minCost.Value);

            if (maxCost.HasValue)
                query = query.Where(g => g.Cost <= maxCost.Value);
        }


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

    /// <summary>
    ///     Применяет сортировку к запросу играм.
    /// </summary>
    /// <param name="query">Запрос для сортировки.</param>
    /// <param name="order">Параметр сортировки.</param>
    /// <returns>Отсортированный запрос игр.</returns>
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