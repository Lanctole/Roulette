using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;

namespace ShikimoriSharp
{
    public class ApiClient
    {
        private const int RPS = 5; 
        private const int RPM = 90; 

        public readonly TokenBucket BucketRpm = new("M", RPM, TimeSpan.FromMinutes(1.05).TotalMilliseconds);
        public readonly TokenBucket BucketRps = new("S", RPS, TimeSpan.FromSeconds(1).TotalMilliseconds);

        private readonly ClientSettings _settings;
        private Func<AccessToken, RequestManager> Request => x => new RequestManager(BucketRps, BucketRpm, _settings, x, RequestTokenRefreshing);

        public event Action<AccessToken> OnNewToken;

        public AuthorizationManager AuthorizationManager { get; }

        public ApiClient(ClientSettings settings)
        {
            _settings = settings;
            AuthorizationManager = new AuthorizationManager(settings, RefreshRequest);
        }

        private async Task<AccessToken> RequestTokenRefreshing(AccessToken expiredToken)
        {
            var newToken = await AuthorizationManager.RefreshAccessToken(expiredToken);
            OnNewToken?.Invoke(newToken);
            return newToken;
        }

        private async Task<AccessToken> RefreshRequest(string dest, HttpContent content)
        {
            var requester = new RequestManager(BucketRps, BucketRpm, _settings, null, null);
            return await requester.ResponseAsType<AccessToken>(dest, "POST", content);
        }

        public async Task<TResult> RequestForm<TResult>(string destination, HttpContent settings, AccessToken token = null, string method = "GET")
        {
            Console.WriteLine("We in public async Task<TResult> RequestForm<TResult>(string destination, HttpContent settings, AccessToken token = null, string method = \"GET\")");
            var requester = Request(token);
            return await requester.ResponseAsType<TResult>(destination, method, settings);
        }

        public async Task<TResult> RequestForm<TResult>(string destination, AccessToken token = null, string method = "GET")
        {
            return await RequestForm<TResult>(destination, null, token, method);
        }
    }
}