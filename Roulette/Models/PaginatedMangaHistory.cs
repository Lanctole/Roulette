namespace Roulette.Models;

public class PaginatedMangaHistory
{
    public List<UserMangaChoice> Items { get; set; }
    public int TotalCount { get; set; }
}