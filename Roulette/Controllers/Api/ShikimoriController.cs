using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Models.Shiki;
using Roulette.Services;
using ShikimoriSharp;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Information;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api
{
    // TODO проанализировать нужно ли создавать фабрику
    /// <summary>
    /// Базовый контроллер для взаимодействия с Shikimori API.
    /// </summary>
    //[Route("api/[controller]")]
    [ApiController]
    public class ShikimoriController(ShikimoriApiConnectorService apiConnectorService, ShikiDataHelper shikiDataHelper) : ControllerBase
    {
        protected readonly ShikimoriApiConnectorService _apiConnectorService = apiConnectorService;
        protected readonly ShikiDataHelper ShikiDataHelper = shikiDataHelper;
    }
}
