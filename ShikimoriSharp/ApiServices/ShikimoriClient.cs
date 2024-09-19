using System.Net.Http;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

public class ShikimoriClient
{
    public ApiClient Client { get; }
    public Animes Animes { get; }
    public Genres Genres { get; }
    public Mangas Mangas { get; }
    public Publishers Publishers { get; }
    public Ranobes Ranobes { get; }
    public Studios Studios { get; }

    public ShikimoriClient(ClientSettings settings, HttpClient httpClient, ILogger<ApiBase> logger)
    {
        Client = new ApiClient(settings, httpClient);
        Animes = new Animes(Client, logger);
        Genres = new Genres(Client, logger);
        Mangas = new Mangas(Client, logger);
        Publishers = new Publishers(Client, logger);
        Ranobes = new Ranobes(Client, logger);
        Studios = new Studios(Client, logger);
    }
}