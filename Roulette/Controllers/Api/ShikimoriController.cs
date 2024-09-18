using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Services;

namespace Roulette.Controllers.Api;

/// <summary>
/// Базовый контроллер для работы с Shikimori API.
/// Этот контроллер предоставляет общие зависимости и методы для взаимодействия с Shikimori API,
/// такие как сервис для доступа к API и вспомогательные методы для обработки данных.
/// </summary>
[ApiController]
public class ShikimoriController
    (ShikimoriApiConnectorService apiConnectorService, ShikiDataHelper shikiDataHelper) : ControllerBase
{
    protected readonly ShikimoriApiConnectorService _apiConnectorService = apiConnectorService;
    protected readonly ShikiDataHelper ShikiDataHelper = shikiDataHelper;
}