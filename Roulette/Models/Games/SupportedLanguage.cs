namespace Roulette.Models.Games
{
    public class SupportedLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Game> Games { get; set; }
    }
}
