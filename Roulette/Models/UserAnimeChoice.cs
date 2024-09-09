using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roulette.DTOs;

namespace Roulette.Models
{
    public class UserAnimeChoice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }
        public long AnimeId { get; set; }
        public AnimeDto? Anime { get; set; }
        public DateTime ChosenAt { get; set; } = DateTime.UtcNow;
    }
}
