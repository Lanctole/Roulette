using System;
using System.Net.Http;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Information;

namespace ShikimoriSharp
{
    public class ShikimoriClient
    {
        public ApiClient Client { get; }
        public Animes Animes { get; }
        public Constants Constants { get; }
        public Genres Genres { get; }
        public Mangas Mangas { get; }
        public Publishers Publishers { get; }
        public Ranobes Ranobes { get; }
        public Studios Studios { get; }

        public ShikimoriClient(ClientSettings settings)
        {
            Client = new ApiClient(settings);
            Animes = new Animes(Client);
            Constants = new Constants(Client);
            Genres = new Genres(Client);
            Mangas = new Mangas(Client);
            Publishers = new Publishers(Client);
            Ranobes = new Ranobes(Client);
            Studios = new Studios(Client);

            Console.WriteLine("ShikimoriClient initialized");
        }
    }
}