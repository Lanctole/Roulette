using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Exceptions;

namespace ShikimoriSharp
{
    public class RequestManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly TokenBucket _bucketRpm;
        private readonly TokenBucket _bucketRps;
        private readonly ClientSettings _settings;
        private readonly Func<AccessToken, Task<AccessToken>> _refresh;
        private AccessToken _token;

        public RequestManager(TokenBucket bucketRps, TokenBucket bucketRpm, ClientSettings settings, AccessToken token, Func<AccessToken, Task<AccessToken>> refresh)
        {
            _bucketRps = bucketRps;
            _bucketRpm = bucketRpm;
            _settings = settings;
            _token = token;
            _refresh = refresh;
        }

        private async Task<HttpResponseMessage> Response(string dest, string method, HttpContent data)
        {
            await _bucketRpm.TokenRequest();
            await _bucketRps.TokenRequest();

            var request = new HttpRequestMessage(new HttpMethod(method), dest)
            {
                Content = data
            };
            request.Headers.TryAddWithoutValidation("User-Agent", _settings.ClientName);

            if (_token != null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"{_token.TokenType} {_token.Access_Token}");
            }

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (_refresh == null)
            {
                throw new Exception($"An error occurred while token refreshing: {response.StatusCode}");
            }

            _token = await _refresh(_token);
            throw new HttpRequestException($"{response.StatusCode}");
        }

        public async Task<string> ResponseExecutor(string dest, string method, HttpContent data)
        {
            Console.WriteLine("Entering ResponseExecutor method.");
            var policy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)));

            HttpResponseMessage response;

            try
            {
                Console.WriteLine($"Response try: ");
                response = await policy.ExecuteAsync(() => Response(dest, method, data));
                Console.WriteLine($"Response received: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during policy execution: {ex.Message}");
                throw;
            }

            if (response == null)
            {
                Console.WriteLine("Response is null after retries.");
                throw new Exception("No response received after retries.");
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.UnprocessableEntity:
                    Console.WriteLine("Unprocessable Entity Error");
                    throw new UnprocessableEntityException();
                case HttpStatusCode.Forbidden:
                    Console.WriteLine("Forbidden Error");
                    throw new ForbiddenException();
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Unsuccessful request: {response.StatusCode} | {response.ReasonPhrase}");
                throw new Exception($"Unsuccessful request: {response.StatusCode} | {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content successfully read.");
            return content;
        }


        public async Task<TResult> ResponseAsType<TResult>(string dest, string method, HttpContent data)
        {
            Console.WriteLine("We in  public async Task<TResult> ResponseAsType<TResult>(string dest, string method, HttpContent data)");
            var response = await ResponseExecutor(dest, method, data);
            return JsonConvert.DeserializeObject<TResult>(response);
        }
    }
}