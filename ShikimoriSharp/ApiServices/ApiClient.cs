using System;
using System.Net.Http;
using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices
{
    public class ApiClient
    {
        private const int RPS = 5;
        private const int RPM = 90;

        public readonly TokenBucket BucketRpm = new("M", RPM, TimeSpan.FromMinutes(1.05).TotalMilliseconds);
        public readonly TokenBucket BucketRps = new("S", RPS, TimeSpan.FromSeconds(1).TotalMilliseconds);

        private readonly ClientSettings _settings;
        public readonly HttpClient _httpClient;
        private Func<AccessToken, RequestManager> Request => x => new RequestManager(_httpClient,BucketRps, BucketRpm, _settings, x, RequestTokenRefreshing);

        public event Action<AccessToken> OnNewToken;

        public AuthorizationManager AuthorizationManager { get; }

        public ApiClient(ClientSettings settings, HttpClient httpClient)
        {
            _settings = settings;
            AuthorizationManager = new AuthorizationManager(settings, RefreshRequest, httpClient.BaseAddress);
            _httpClient = httpClient;
            
        }

        private async Task<AccessToken> RequestTokenRefreshing(AccessToken expiredToken)
        {
            var newToken = await AuthorizationManager.RefreshAccessToken(expiredToken);
            OnNewToken?.Invoke(newToken);
            return newToken;
        }

        private async Task<AccessToken> RefreshRequest(string dest, HttpContent content)
        {
            var requester = new RequestManager(_httpClient,BucketRps, BucketRpm, _settings, null, null);
            return await requester.ResponseAsType<AccessToken>(dest, "POST", content);
        }

        public async Task<TResult> RequestForm<TResult>(string destination, HttpContent settings, AccessToken token = null, string method = "GET")
        {
            var requester = Request(token);
            return await requester.ResponseAsType<TResult>(destination, method, settings);
        }

        public async Task<TResult> RequestForm<TResult>(string destination, AccessToken token = null, string method = "GET")
        {
            return await RequestForm<TResult>(destination, null, token, method);
        }
    }
}