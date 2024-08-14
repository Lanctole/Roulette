using System;
using System.Net.Http;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Information;

namespace ShikimoriSharp.ApiServices
{
    public class ShikimoriClient
    {
        public ApiClient Client { get; }
        public Animes Animes { get; }
        public Genres Genres { get; }
        public Mangas Mangas { get; }
        public Publishers Publishers { get; }
        public Ranobes Ranobes { get; }
        public Studios Studios { get; }

        public ShikimoriClient(ClientSettings settings, HttpClient httpClient)
        {
            Client = new ApiClient(settings,httpClient);
            Animes = new Animes(Client);
            Genres = new Genres(Client);
            Mangas = new Mangas(Client);
            Publishers = new Publishers(Client);
            Ranobes = new Ranobes(Client);
            Studios = new Studios(Client);
        }
    }
}