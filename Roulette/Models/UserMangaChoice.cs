using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roulette.DTOs;

namespace Roulette.Models
{
    public class UserMangaChoice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }
        public long MangaId { get; set; }
        public MangaDto? Manga { get; set; }
        public DateTime ChosenAt { get; set; } = DateTime.UtcNow;
    }
}
