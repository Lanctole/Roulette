using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ShikimoriApiConnector _apiConnector;

        public ApiTestController(ShikimoriApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        [HttpGet("genres")]
        public IActionResult GetGenres()
        {
            var genres = _apiConnector.GetGenres();
            return Ok(genres);
        }


    }
}
