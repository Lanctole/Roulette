using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Roulette.Components.Account;

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5)
    };

    private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative)) uri = navigationManager.ToBaseRelativePath(uri);
        uri = EnsureHttps(uri);
        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException(
            $"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        newUri = EnsureHttps(newUri);
        RedirectTo(newUri);
    }

    [DoesNotReturn]
    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        uri = EnsureHttps(uri);
        RedirectTo(uri);
    }

    [DoesNotReturn]
    public void RedirectToCurrentPage()
    {
        RedirectTo(CurrentPath);
    }

    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
    {
        RedirectToWithStatus(CurrentPath, message, context);
    }

    private string EnsureHttps(string uri)
    {
        var absoluteUri = navigationManager.ToAbsoluteUri(uri);
        if (absoluteUri.Scheme != Uri.UriSchemeHttps)
        {
            var builder = new UriBuilder(absoluteUri) { Scheme = Uri.UriSchemeHttps, Port = -1 };
            return builder.Uri.ToString();
        }

        return uri;
    }

    public string GetUriWithQueryParameters(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriBuilder = new UriBuilder(navigationManager.ToAbsoluteUri(uri));

        uriBuilder.Scheme = "https";
        uriBuilder.Port = -1;

        if (queryParameters != null && queryParameters.Count > 0)
        {
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var param in queryParameters)
                if (param.Value != null)
                    query[param.Key] = param.Value.ToString();
            uriBuilder.Query = query.ToString();
        }

        return uriBuilder.ToString();
    }
}