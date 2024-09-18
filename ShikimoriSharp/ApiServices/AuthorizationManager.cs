﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

public class AuthorizationManager
{
    private readonly string _tokenUrl;
    private readonly ClientSettings _settings;
    private readonly Func<string, HttpContent, Task<AccessToken>> _refreshFunc;

    public AuthorizationManager(ClientSettings settings, Func<string, HttpContent, Task<AccessToken>> refreshFunc, Uri baseUrl)
    {
        _settings = settings;
        _refreshFunc = refreshFunc;
        _tokenUrl = new Uri(baseUrl, "oauth/token").ToString();
    }

    public async Task<AccessToken> GetAccessToken(string authCode)
    {
        return await GetAccessTokenRequest(new MultipartFormDataContent
        {
            { new StringContent("authorization_code"), "grant_type" },
            { new StringContent(_settings.ClientId), "client_id" },
            { new StringContent(_settings.ClientSecret), "client_secret" },
            { new StringContent(authCode), "code" },
            { new StringContent(_settings.RedirectUrl), "redirect_uri" }
        });
    }

    public async Task<AccessToken> RefreshAccessToken(AccessToken oldToken)
    {
        return await GetAccessTokenRequest(new MultipartFormDataContent
        {
            { new StringContent("refresh_token"), "grant_type" },
            { new StringContent(_settings.ClientId), "client_id" },
            { new StringContent(_settings.ClientSecret), "client_secret" },
            { new StringContent(oldToken.RefreshToken), "refresh_token" }
        });
    }

    private async Task<AccessToken> GetAccessTokenRequest(HttpContent stringData)
    {
        return await _refreshFunc(_tokenUrl, stringData);
    }
}