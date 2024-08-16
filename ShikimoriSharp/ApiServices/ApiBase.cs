using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using Version = ShikimoriSharp.Enums.Version;

namespace ShikimoriSharp.ApiServices
{
    public abstract class ApiBase
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseUrl;

        protected ApiBase(Version version, ApiClient apiClient)
        {
            Version = version;
            _apiClient = apiClient;
        }

        public Version Version { get; }
        private string Site => new Uri(_apiClient._httpClient.BaseAddress, $"api/{GetApiVersionPath()}").ToString();

        private string GetApiVersionPath()
        {
            return Version switch
            {
                Version.v1 => "",
                _ => Version + "/"
            };
        }

        private static MultipartFormDataContent SerializeToHttpContent<T>(T obj)
        {
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
            var settingsContent = SerializeToHttpContent(settings);
                return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", settingsContent, token, method);
        }

        public async Task<TResult> RequestAsync<TResult>(string apiMethod, AccessToken token = null, string method = "GET")
        {
            return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", token);
        }
    }
}
