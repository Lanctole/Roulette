using Microsoft.AspNetCore.Mvc;
using Roulette.Services;
using ShikimoriSharp.Enums;

namespace Roulette.Controllers.Api;

/// <summary>
///     Контроллер для работы с мангой.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MangaController : ControllerBase
{

    private readonly MangaService _mangaService;

    public MangaController(MangaService mangaService)
    {
        _mangaService = mangaService;
    }

    /// <summary>
    ///     Возвращает список манг, соответствующих указанным критериям.
    /// </summary>
    /// <param name="score">Минимальная оценка манги [0-10].</param>
    /// <param name="kind">Формат распространения манги [manga, manhwa, manhua, light_novel, novel, one_shot, doujin].</param>
    /// <param name="season">Время выхода манги [summer_2017, 2016].</param>
    /// <param name="publisher">Издатель манги.</param>
    /// <param name="genre">Жанр манги.</param>
    /// <param name="censored">Прятать ли анцензоред [true, false].</param>
    /// <param name="search">Параметр поиска для фильтрации манги.</param>
    /// <param name="status">Статус манги. Может быть 'вышло', 'выходит' или другое значение.</param>
    /// <param name="order">
    ///     Порядок сортировки [id, id_desc, ranked, kind, popularity, name, aired_on, episodes, status,
    ///     random].
    /// </param>
    /// <param name="limit">Количество возвращаемых результатов.</param>
    /// <param name="page">Номер страницы для пагинации.</param>
    /// <returns>
    ///     Список манг, удовлетворяющих указанным условиям.
    ///     Возвращает код состояния 200 (OK) при успешном выполнении.
    /// </returns>
    [HttpGet("mangas")]
    public async Task<IActionResult> GetMangas(
        [FromQuery] int? score = null,
        [FromQuery] string? kind = null,
        [FromQuery] string? season = null,
        [FromQuery] string? publisher = null,
        [FromQuery] string? genre = null,
        [FromQuery] bool? censored = null,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] Order? order = null,
        [FromQuery] int limit = 5,
        [FromQuery] int? page = 1)
    {
        if (limit <= 0) return BadRequest("Параметр 'limit' должен быть положительным числом.");

        if (page <= 0) return BadRequest("Параметр 'page' должен быть положительным числом.");

        try
        {
            var filters = new Dictionary<string, string>();
            if (score.HasValue) filters.Add("score", score.Value.ToString());
            if (!string.IsNullOrEmpty(kind)) filters.Add("kind", $"\"{kind}\"");
            if (!string.IsNullOrEmpty(season)) filters.Add("season", $"\"{season}\"");
            if (!string.IsNullOrEmpty(publisher)) filters.Add("publisher", $"\"{publisher}\"");
            if (!string.IsNullOrEmpty(genre)) filters.Add("genre", $"\"{genre}\"");
            if (censored.HasValue) filters.Add("censored", censored.Value.ToString().ToLower());
            if (!string.IsNullOrEmpty(search)) filters.Add("search", $"\"{search}\"");
            if (!string.IsNullOrEmpty(status)) filters.Add("status", $"\"{status}\"");
            if (order.HasValue) filters.Add("order", $"{order.Value.ToString().ToLower()}");
            filters.Add("limit", limit.ToString());
            filters.Add("page", page.ToString());

            var filterString = string.Join(", ", filters.Select(f => $"{f.Key}: {f.Value}"));
            var mangas = await _mangaService.GetMangasAsync(filterString);
            return Ok(mangas);
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(503, $"Ошибка запроса к API: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }
}