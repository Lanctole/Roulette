using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Models.Shiki;
using Roulette.Services;
using ShikimoriSharp;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Information;

namespace Roulette.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        private readonly ShikimoriApiConnectorService _apiConnectorService;
        private readonly ShikiDataService _shikiDataService;

        public ApiTestController(ShikimoriApiConnectorService apiConnectorService, ShikiDataService shikiDataService)
        {
            _apiConnectorService = apiConnectorService;
            _shikiDataService = shikiDataService;
            _shikiDataService = shikiDataService;
        }

        [HttpGet("genres")]
        public IActionResult GetGenres()
        {
            var genres = _apiConnectorService.GetGenres();
            var transformedGenres = _shikiDataService.TransformGenres(genres);
            return Ok(transformedGenres);
        }

        [HttpGet("studios")]
        public IActionResult GetStudios()
        {
            var studios = _apiConnectorService.GetStudios();
            return Ok(studios);
        }



    }
}
