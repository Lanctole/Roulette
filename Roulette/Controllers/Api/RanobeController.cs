using Microsoft.AspNetCore.Mvc;
using Roulette.Services;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api
{
    /// <summary>
    /// Контроллер для работы с ranobe
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RanobeController(ShikimoriApiConnectorService apiConnectorService, ShikiDataService shikiDataService) : ShikimoriController(apiConnectorService, shikiDataService)
    {

        /// <summary>
        /// Возвращает список light novel.
        /// </summary>
        /// <param name="score">Минимальная оценка [0-10].</param>
        /// <param name="season">Время выхода [summer_2017, 2016]</param>
        /// <param name="publisher">Издатель</param>
        /// <param name="genre">Жанр</param>
        /// <param name="censored">Прятать ли анцензоред [true, false]</param>
        /// <param name="search">Параметр поиска</param>
        /// <param name="status">Статус. Вышло/выходит. Может быть убрано</param>
        /// <param name="order">[id, id_desc, ranked, kind, popularity, name, aired_on, episodes, status, random]</param>
        /// <param name="limit">Количество возвращаемых результатов</param>
        /// <param name="page">Страницы</param>
        /// <returns>Список произведений удовлетворяющих условиям.</returns>
        [HttpGet("ranobes")]
        public IActionResult GetMangas(
            [FromQuery] int? score = null,
            [FromQuery] string? season = null,
            [FromQuery] int[]? publisher = null,
            [FromQuery] string? genre = null,
            [FromQuery] bool? censored = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] Order? order = null,
            [FromQuery] int limit = 5,
            [FromQuery] int? page = 1)
        {
            var settings = new RanobeRequestSettings
            {
                order = order,
                score = score,
                season = season,
                publisher = publisher,
                genre = genre,
                censored = censored,
                search = search,
                status = status,
                limit = limit,
                page = page
            };
            var ranobes = _apiConnectorService.GetRanobes(settings);
            return Ok(ranobes);
        }
    }
}
