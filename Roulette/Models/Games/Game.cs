using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roulette.Models.Games
{
    public class Game
    {
        [Key]
        public long AppID { get; set; } 
        public string Name { get; set; } = string.Empty;
        public double Cost { get; set; }
        public DateTime ReleaseDate { get; set; } 
        public string ShortDescription { get; set; } = string.Empty;
        public string HeaderImage { get; set; } = string.Empty;
        public int SteamScore { get; set; } 
        public int MetacriticScore { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<SupportedLanguage> SupportedLanguages { get; set; }
    }
}
