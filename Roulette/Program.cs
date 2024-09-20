using System.Reflection;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Roulette.Components;
using Roulette.Components.Account;
using Roulette.Data;
using Roulette.Helpers;
using Roulette.Models;
using Roulette.Services;

namespace Roulette;

/// <summary>
/// Главный класс
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа в приложение
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        ConfigureHttpClients(builder);
        ConfigureServices(builder);
        ConfigureAuthentication(builder);
        ConfigureRedisCache(builder);
        ConfigureSwagger(builder);
        ConfigureDbContext(builder);
        ConfigureLogging(builder);
        ConfigureSmtpSettings(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddAntDesign();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<ShikimoriApiConnectorService>();
        builder.Services.AddScoped<ShikiDataHelper>();
        builder.Services.AddSingleton<ApiClientService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddScoped<GameService>();
        builder.Services.AddScoped<GameGenreService>();
        builder.Services.AddScoped<GameLanguageService>();
        builder.Services.AddScoped<UserChoiceHistoryService>();
        builder.Services.AddScoped<BugReportService>();
    }

    private static void ConfigureHttpClients(WebApplicationBuilder builder)
    {
        var shikimoriBaseUrl = Environment.GetEnvironmentVariable("Shikimori:BaseUrl") ??
                               builder.Configuration.GetSection("Shikimori:BaseUrl").Value;
        builder.Services.AddHttpClient<ShikimoriApiConnectorService>(client =>
            {
                client.BaseAddress = new Uri(shikimoriBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).SetHandlerLifetime(TimeSpan.FromMinutes(1))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

        var apiUrl = Environment.GetEnvironmentVariable("ApiBaseAddress") ??
                     builder.Configuration["ApiBaseAddress"];
        builder.Services.AddHttpClient<ApiClientService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(1))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddYandex(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("Authentication:Yandex:ClientId") ??
                                   builder.Configuration["Authentication:Yandex:ClientId"];
                options.ClientSecret = Environment.GetEnvironmentVariable("Authentication:Yandex:ClientSecret") ??
                                       builder.Configuration["Authentication:Yandex:ClientSecret"];
                options.CallbackPath = new PathString("/Account/SingInYandex");
            })
            .AddIdentityCookies();

        builder.Services.AddIdentityCore<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
            .AddDefaultTokenProviders();
    }

    private static void ConfigureRedisCache(WebApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Environment.GetEnvironmentVariable("Redis:Configuration") ??
                                    builder.Configuration.GetSection("Redis:Configuration").Value;
            options.InstanceName = Environment.GetEnvironmentVariable("Redis:InstanceName") ??
                                   builder.Configuration.GetSection("Redis:InstanceName").Value;
        });
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Roulette API",
                Version = "v1",
                Description = "An ASP.NET Core Web API for ROULETTE"
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
        });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void ConfigureDbContext(WebApplicationBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ??
                               builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }

    private static void ConfigureSmtpSettings(WebApplicationBuilder builder)
    {
        builder.Services.Configure<SmtpSettings>(options =>
        {
            options.Host = Environment.GetEnvironmentVariable("Smtp:Host") ?? builder.Configuration["Smtp:Host"];
            options.Port = int.Parse(Environment.GetEnvironmentVariable("Smtp:Port") ??
                                     builder.Configuration["Smtp:Port"] ?? "2525");
            options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("Smtp:EnableSsl") ??
                                           builder.Configuration["Smtp:EnableSsl"] ?? "true");
            options.Username = Environment.GetEnvironmentVariable("Smtp:Username") ?? builder.Configuration["Smtp:Username"];
            options.Password = Environment.GetEnvironmentVariable("Smtp:Password") ?? builder.Configuration["Smtp:Password"];
            options.FromEmail = Environment.GetEnvironmentVariable("Smtp:FromEmail") ?? builder.Configuration["Smtp:FromEmail"];
        });
        builder.Services.AddTransient<IEmailSender, EmailSender>();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Roulette API v1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            //app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        app.UseAntiforgery();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapAdditionalIdentityEndpoints();
    }
}