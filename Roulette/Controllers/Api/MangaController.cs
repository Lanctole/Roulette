using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Services;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api;

/// <summary>
///     Контроллер для работы с мангой.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MangaController
    (ShikimoriApiConnectorService apiConnectorService, ShikiDataHelper shikiDataHelper) : ShikimoriController(
        apiConnectorService, shikiDataHelper)
{
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

        var settings = new MangaRequestSettings
        {
            order = order,
            score = score,
            kind = kind,
            season = season,
            publisher = publisher,
            genre = genre,
            censored = censored,
            search = search,
            status = status,
            limit = limit,
            page = page
        };
        try
        {
            var mangas = await _apiConnectorService.GetMangas(settings);
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

    /// <summary>
    ///     Возвращает мангу по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор манги. Должен быть положительным числом.</param>
    /// <returns>
    ///     Детальная информация о манге с указанным ID.
    ///     В случае внутренней ошибки сервера возвращает код состояния 500 (Internal Server Error).
    /// </returns>
    [HttpGet("mangas/{id}")]
    public async Task<IActionResult> GetMangaById(long id)
    {
        if (id <= 0) return BadRequest("Идентификатор манги должен быть положительным числом.");

        try
        {
            var manga = await _apiConnectorService.GetMangaById(id);
            return Ok(manga);
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

    /// <summary>
    ///     Возвращает список издателей манги.
    /// </summary>
    /// <returns>
    ///     Список издателей.
    ///     Возвращает код состояния 200 (OK) при успешном выполнении.
    ///     В случае внутренней ошибки сервера возвращает код состояния 500 (Internal Server Error).
    /// </returns>
    [HttpGet("publishers")]
    public async Task<IActionResult> GetPublishers()
    {
        try
        {
            var publishers = await _apiConnectorService.GetPublishers();

            return Ok(publishers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}