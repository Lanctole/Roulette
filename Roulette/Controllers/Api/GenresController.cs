using Microsoft.AspNetCore.Mvc;
using Roulette.Services;

namespace Roulette.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly GenreService _genreService;

    public GenresController(GenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet("animeGenres")]
    public async Task<IActionResult> GetAnimeGenres()
    {
        try
        {
            var genres = await _genreService.GetAnimeGenresAsync();
            return Ok(genres);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("mangaGenres")]
    public async Task<IActionResult> GetMangaGenres()
    {
        try
        {
            var genres = await _genreService.GetMangaGenresAsync();
            return Ok(genres);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
