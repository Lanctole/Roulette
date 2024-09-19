namespace Roulette.Models;

public class PaginatedAnimeHistory
{
    public List<UserAnimeChoice> Items { get; set; }
    public int TotalCount { get; set; }
}