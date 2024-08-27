using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Services;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api
{
    /// <summary>
    /// Контроллер для работы с anime
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController(ShikimoriApiConnectorService apiConnectorService, ShikiDataHelper shikiDataHelper) : ShikimoriController(apiConnectorService, shikiDataHelper)
    {

        /// <summary>
        /// Получить жанры.
        /// </summary>
        /// <returns>Возвращает список жанров, сгрупированных по названию.</returns>
        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            try
            {
                var genres = await _apiConnectorService.GetGenres();
                var transformedGenres = ShikiDataHelper.TransformGenres(genres);
                return Ok(transformedGenres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Возвращает список студий.
        /// </summary>
        /// <returns>Список студий</returns>
        [HttpGet("studios")]
        public async Task<IActionResult> GetStudios()
        {
            try
            {
                var studios = await _apiConnectorService.GetStudios();
                return Ok(studios);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Возвращает список аниме.
        /// </summary>
        /// <param name="score">Минимальная оценка [0-10].</param>
        /// <param name="rating">Рейтинг [ g, pg, pg_13, r, r_plus, rx] </param>
        /// <param name="kind">Формат распространения [tv, movie, ova, ona, special, tv_special, music, pv, cm, tv_13, tv_24, tv_48]</param>
        /// <param name="season">Время выхода [summer_2017, 2016]</param>
        /// <param name="studio">Студия</param>
        /// <param name="genre">Жанр</param>
        /// <param name="censored">Прятать ли анцензоред [true, false]</param>
        /// <param name="search">Параметр поиска</param>
        /// <param name="duration">Продолжительность S- меньше 10 минут, D- меньше 30 минут, F- больше 30 минут. [S, D, F]</param>
        /// <param name="status">Статус. Вышло/выходит. Может быть убрано</param>
        /// <param name="order">[id, id_desc, ranked, kind, popularity, name, aired_on, episodes, status, random]</param>
        /// <param name="limit">Количество возвращаемых результатов</param>
        /// <param name="page">Страницы</param>
        /// <returns>Список произведений удовлетворяющих условиям.</returns>
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
            var settings = new AnimeRequestSettings
            {
                order = order,
                score = score,
                rating = rating,
                kind = kind,
                season = season,
                studio = studio,
                genre = genre,
                censored = censored,
                search = search,
                duration = duration,
                status = status,
                limit = limit,
                page = page
            };
            var animes = await _apiConnectorService.GetAnimes(settings);
            return Ok(animes);
        }

        /// <summary>
        /// Возвращает аниме по его ID.
        /// </summary>
        /// <param name="id">ID аниме</param>
        /// <returns>Детальная информация об аниме</returns>
        [HttpGet("animes/{id}")]
        public async Task<IActionResult> GetAnimeById(long id)
        {
            try
            {
                var anime = await _apiConnectorService.GetAnimeById(id);
                if (anime == null)
                {
                    return NotFound($"Anime with ID {id} not found.");
                }
                return Ok(anime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
