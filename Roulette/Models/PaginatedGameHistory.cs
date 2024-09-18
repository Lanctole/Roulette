namespace Roulette.Models;

public class PaginatedGameHistory
{
    public List<UserGameChoice> Items { get; set; }
    public int TotalCount { get; set; }
}