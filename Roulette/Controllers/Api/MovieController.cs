using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Services;

namespace Roulette.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        protected readonly MovieApiService _movieService;

        public MovieController(MovieApiService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _movieService.GetGenresAsync();
            return Ok(genres);
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _movieService.GetCountriesAsync();
            return Ok(countries);
        }
        
        //[HttpGet("movies")]
        //public async Task<IActionResult> GetMovies()
        //{
        //    var movies = await _movieService.GetMoviesAsync();
        //    return Ok(movies);
        //}

    }
}
