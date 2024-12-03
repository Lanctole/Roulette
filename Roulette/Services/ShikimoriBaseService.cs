using Microsoft.Extensions.Caching.Distributed;
using Roulette.Data;

namespace Roulette.Services;

public class ShikimoriBaseService
{
    protected readonly IDistributedCache _cache;
    protected readonly ApplicationDbContext _context;
    protected readonly ILogger _logger;

    protected ShikimoriBaseService(IDistributedCache cache, ApplicationDbContext context, ILogger logger)
    {
        _cache = cache;
        _context = context;
        _logger = logger;
    }

    public async Task<List<T>> GetOrFetchDataAsync<T>(
        IEnumerable<long> ids,
        Func<IEnumerable<long>, Task<List<T>>> fetchFromApi,
        Func<long, Task<bool>> checkInDatabase,
        Func<long, Task<T>> fetchFromCache
    )
    {
        var missingIds = new List<long>();
        var results = new List<T>();

        foreach (var id in ids)
        {
            var cachedData = await fetchFromCache(id);
            if (cachedData != null)
            {
                results.Add(cachedData);
                continue;
            }

            var existsInDb = await checkInDatabase(id);
            if (!existsInDb)
            {
                missingIds.Add(id);
            }
        }

        if (missingIds.Any())
        {
            var fetchedData = await fetchFromApi(missingIds);
            results.AddRange(fetchedData);
        }

        return results;
    }

    protected string CreateFilterString(IEnumerable<long> ids)
    {
        return $"ids: [{string.Join(",", ids)}]";
    }
}