using Microsoft.Extensions.Caching.Distributed;

namespace Roulette.Services;

public interface IImageCacheService
{
    Task SaveImageToCacheAsync(string imageUrl);
    Task<string?> GetImageFromCacheAsync(string imageUrl);
}

public class ImageCacheService : IImageCacheService
{
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;

    public ImageCacheService(IDistributedCache cache, HttpClient httpClient)
    {
        _cache = cache;
        _httpClient = httpClient;
    }

    public async Task SaveImageToCacheAsync(string imageUrl)
    {
        var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(7));
        await _cache.SetAsync(imageUrl, imageBytes, options);
    }

    public async Task<string?> GetImageFromCacheAsync(string imageUrl)
    {
        var imageBytes = await _cache.GetAsync(imageUrl);

        if (imageBytes == null)
            return null;

        var base64Image = Convert.ToBase64String(imageBytes);
        return $"data:image/jpeg;base64,{base64Image}";
    }
}