using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Models.Shiki;
using Roulette.Services;
using ShikimoriSharp;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Information;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api
{
    /// <summary>
    /// Базовый контроллер для взаимодействия с Shikimori API.
    /// </summary>
    //[Route("api/[controller]")]
    [ApiController]
    public class ShikimoriController : ControllerBase
    {
        protected readonly ShikimoriApiConnectorService _apiConnectorService;
        protected readonly ShikiDataService _shikiDataService;
       
        public ShikimoriController(ShikimoriApiConnectorService apiConnectorService, ShikiDataService shikiDataService)
        {
            _apiConnectorService = apiConnectorService;
            _shikiDataService = shikiDataService;
            _shikiDataService = shikiDataService;
        }

    }
}
