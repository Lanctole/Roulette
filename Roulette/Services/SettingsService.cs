using Roulette.Models;

namespace Roulette.Services
{
    public class SettingsService
    {
        private readonly IConfiguration _configuration;

        public SettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ShikimoriSettings GetShikimoriSettings()
        {
            return _configuration.GetSection("Shikimori").Get<ShikimoriSettings>();
        }
    }
}
