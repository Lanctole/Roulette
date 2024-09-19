using Microsoft.AspNetCore.Identity;
using Roulette.DTOs;

namespace Roulette.Models;

public class UserRanobeChoice
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public IdentityUser? User { get; set; }
    public long RanobeId { get; set; }
    public RanobeDto? Ranobe { get; set; }
    public DateTime ChosenAt { get; set; } = DateTime.UtcNow;
}