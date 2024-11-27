using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Roulette.Helpers;
using Roulette.Services;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Roulette.Controllers.Api;

/// <summary>
///     Контроллер для работы с данными об аниме.
///     Этот контроллер предоставляет конечные точки API для получения информации о жанрах, студиях, аниме и подробностей
///     об отдельных аниме.
///     Он наследует от <see cref="ShikimoriController" /> и использует его для взаимодействия с Shikimori API.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AnimeController : ControllerBase
{
    private readonly AnimeService _animeService;

    public AnimeController(AnimeService animeService)
    {
        _animeService = animeService;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        string answer = "";
        using (var client = new System.Net.Http.HttpClient())
        {
            var request = new System.Net.Http.HttpRequestMessage();
            //request.RequestUri = new Uri("http://shikimoriservice:8080/Status");
            //request.RequestUri = new Uri("http://shikimoriservice:8080/Status");
            var graphQLQuery = new
            {
                query = @"{
                      genres(entryType: Anime) {
                        id
                        name
                        russian
                      }
                      genresManga: genres(entryType: Manga) {
                        id
                        name
                        russian
                      }
                    }",
                variables = (object)null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(graphQLQuery),
                Encoding.UTF8,
                "application/json");
            //var response = await client.SendAsync(request);
            var response = await client.PostAsync("http://shikimoriservice:8080/api/GraphQL/query", content);
            answer = await response.Content.ReadAsStringAsync();
        }
        //return Ok(answer);
        //return Ok(JsonSerializer.Deserialize<object>(responseData));
        return Ok(JsonSerializer.Deserialize<object>(answer));
    }


    /// <summary>
    ///     Возвращает список студий.
    /// </summary>
    /// <returns>
    ///     Список студий. Возвращает код состояния 200 (OK) при успешном выполнении.
    ///     В случае ошибки возвращает код состояния 500 (Internal Server Error) с сообщением об ошибке.
    /// </returns>
    //[HttpGet("studios")]
    //public async Task<IActionResult> GetStudios()
    //{
    //    try
    //    {
    //        var studios = await _apiConnectorService.GetStudios();
    //        return Ok(studios);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}


    /// <summary>
    ///     Возвращает список аниме на основе заданных параметров.
    /// </summary>
    /// <param name="score">Минимальная оценка аниме от 0 до 10.</param>
    /// <param name="rating">Рейтинг аниме, например, [g, pg, pg_13, r, r_plus, rx].</param>
    /// <param name="kind">Формат распространения аниме, например, [tv, movie, ova, ona, special].</param>
    /// <param name="season">Сезон выхода аниме, например, [summer_2017, 2016].</param>
    /// <param name="studio">Название студии, выпустившей аниме.</param>
    /// <param name="genre">Жанр аниме.</param>
    /// <param name="censored">Флаг, указывающий, скрывать ли нецензурное содержимое (true/false).</param>
    /// <param name="search">Параметр поиска по названию или описанию.</param>
    /// <param name="duration">Продолжительность аниме: S (менее 10 минут), D (менее 30 минут), F (более 30 минут).</param>
    /// <param name="status">Статус аниме, например, "вышло", "выходит" и т.д.</param>
    /// <param name="order">Параметр сортировки, например, [id, id_desc, ranked, popularity, random].</param>
    /// <param name="limit">Количество возвращаемых результатов. Не может быть отрицательным. По умолчанию 5.</param>
    /// <param name="page">Страница для пагинации. Не может быть меньше 1. По умолчанию 1.</param>
    /// <returns>
    ///     Список аниме, удовлетворяющих заданным условиям.
    ///     Возвращает код состояния 200 (OK) при успешном выполнении.
    ///     В случае ошибки возвращает код состояния 400 (Bad Request) при некорректных входных данных
    ///     или код состояния 500 (Internal Server Error) при внутренней ошибке сервера.
    /// </returns>
    [HttpGet("animes")]
    public async Task<IActionResult> GetAnimes(
        [FromQuery] int? score = null,
        [FromQuery] Rating? rating = null,
        [FromQuery] string? kind = null,
        [FromQuery] string? season = null,
        [FromQuery] string? studio = null,
        [FromQuery] string? genre = null,
        [FromQuery] bool? censored = null,
        [FromQuery] string? search = null,
        [FromQuery] Duration? duration = null,
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
            if (rating.HasValue) filters.Add("rating", $"\"{rating.Value.ToString().ToLower()}\"");
            if (!string.IsNullOrEmpty(kind)) filters.Add("kind", $"\"{kind}\"");
            if (!string.IsNullOrEmpty(season)) filters.Add("season", $"\"{season}\"");
            if (!string.IsNullOrEmpty(studio)) filters.Add("studio", $"\"{studio}\"");
            if (!string.IsNullOrEmpty(genre)) filters.Add("genre", $"\"{genre}\"");
            if (censored.HasValue) filters.Add("censored", censored.Value.ToString().ToLower());
            if (!string.IsNullOrEmpty(search)) filters.Add("search", $"\"{search}\"");
            if (duration.HasValue) filters.Add("duration", $"\"{duration.Value.ToString()}\"");
            if (!string.IsNullOrEmpty(status)) filters.Add("status", $"\"{status}\"");
            if (order.HasValue) filters.Add("order", $"{order.Value.ToString().ToLower()}");
            filters.Add("limit", limit.ToString());
            filters.Add("page", page.ToString());

            var filterString = string.Join(", ", filters.Select(f => $"{f.Key}: {f.Value}"));
            var animes = await _animeService.GetAnimesAsync(filterString);
            return Ok(animes);
            //return Ok(animes);
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
    ///     Возвращает детальную информацию об аниме по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор аниме. Должен быть положительным числом.</param>
    /// <returns>
    ///     Возвращает детальную информацию об аниме с указанным ID.
    ///     В случае внутренней ошибки сервера возвращает код состояния 500 (Internal Server Error).
    /// </returns>
    //[HttpGet("animes/{id}")]
    //public async Task<IActionResult> GetAnimeById(long id)
    //{
    //    if (id <= 0) return BadRequest("Идентификатор аниме должен быть положительным числом.");

    //    try
    //    {
    //        var anime = await _apiConnectorService.GetAnimeById(id);
    //        return Ok(anime);
    //    }
    //    catch (HttpRequestException httpEx)
    //    {
    //        return StatusCode(503, $"Ошибка запроса к API: {httpEx.Message}");
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
    //    }
    //}
}