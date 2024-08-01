namespace Roulette.Models
{
    public class AppSettings
    {
        public ShikimoriSettings Shikimori { get; set; }
    }

    public class ShikimoriSettings
    {
        public string BaseUrl { get; set; }
    }
}
