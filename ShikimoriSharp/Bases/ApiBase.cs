using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShikimoriSharp.Exceptions;

namespace ShikimoriSharp.Bases
{
    public abstract class ApiBase
    {
        private readonly ApiClient _apiClient;

        protected ApiBase(Version version, ApiClient apiClient)
        {
            Version = version;
            _apiClient = apiClient;
        }

        public Version Version { get; }
        private string Site => $"https://shikimori.me/api/{GetApiVersionPath()}";

        protected bool Requires(AccessToken token, IEnumerable<string> scopes)
        {
            var tokenScopes = token.Scope.Split(" ");
            if (!scopes.All(scope => tokenScopes.Contains(scope)))
                throw new NotInScopeException();
            return true;
        }

        private string GetApiVersionPath()
        {
            return Version switch
            {
                Version.v1 => "",
                _ => Version + "/"
            };
        }

        private static HttpContent SerializeToHttpContent<T>(T obj)
        {
            Console.WriteLine("We in private static HttpContent SerializeToHttpContent<T>(T obj)");
            if (obj is null) return null;
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            var content = new MultipartFormDataContent();
            foreach (var field in fields)
            {
                var value = field.GetValue(obj);
                if (value is null) continue;

                var stringContent = value switch
                {
                    bool boolValue => new StringContent(boolValue ? "true" : "false"),
                    _ => new StringContent(value.ToString())
                };

                content.Add(stringContent, field.Name);
            }

            return content;
        }

        public async Task<TResult> RequestAsync<TResult, TSettings>(string apiMethod, TSettings settings,
            AccessToken token = null, string method = "GET")
        {
            try
            {
                Console.WriteLine("We in public async Task<TResult> RequestAsync<TResult, TSettings>");
                var settingsContent = SerializeToHttpContent(settings);
                return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", settingsContent, token, method);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during RequestAsync for {apiMethod}: {ex.Message}");
                throw;
            }
        }

        private static async Task<string> SerializeToJsonAsync(object obj)
        {
            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj));
        }

        public async Task<TResult> SendJsonAsync<TResult>(string apiMethod, object content, AccessToken token,
            string method = "POST")
        {
            try
            {
                var json = new StringContent(await SerializeToJsonAsync(content), Encoding.UTF8, "application/json");
                return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", json, token, method);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during SendJsonAsync for {apiMethod}: {ex.Message}");
                throw;
            }
        }

        public async Task<TResult> RequestAsync<TResult>(string apiMethod, AccessToken token = null, string method = "GET")
        {
            try
            {
                return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during RequestAsync for {apiMethod}: {ex.Message}");
                throw;
            }
        }

        public async Task NoResponseRequestAsync(string apiMethod, AccessToken token, string method = "POST")
        {
            try
            {
                await _apiClient.RequestWithNoResponse($"{Site}{apiMethod}", null, token, method);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during NoResponseRequestAsync for {apiMethod}: {ex.Message}");
                throw;
            }
        }

        public async Task NoResponseRequestAsync<TSettings>(string apiMethod, TSettings settings, AccessToken token,
            string method = "POST")
        {
            try
            {
                var settingsContent = SerializeToHttpContent(settings);
                await _apiClient.RequestWithNoResponse($"{Site}{apiMethod}", settingsContent, token, method);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during NoResponseRequestAsync for {apiMethod}: {ex.Message}");
                throw;
            }
        }
    }

    public enum Version
    {
        v1,
        v2
    }
}
