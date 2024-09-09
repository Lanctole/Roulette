using Games.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Roulette.Models
{
    public class UserGameChoice
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public IdentityUser? User { get; set; }
        public long GameId { get; set; }
        public Game? Game { get; set; }
        public DateTime ChosenAt { get; set; } = DateTime.UtcNow;
    }
}
