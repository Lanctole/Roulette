using System.Reflection;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
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
using AspNet.Security.OAuth.Yandex;


namespace Roulette;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
        builder.Services.AddScoped<IdentityRedirectManager>();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        //builder.Services.AddAntiforgery(options =>
        //{
        //    options.HeaderName = "X-XSRF-TOKEN";
        //});


        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Services.AddAntDesign();

        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient();

        builder.Services.AddHttpClient<ShikimoriApiConnectorService>(client =>
        {
            var baseUrl = builder.Configuration.GetSection("Shikimori")["BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(1)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
             ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
        builder.Services.AddScoped<ShikimoriApiConnectorService>();
        builder.Services.AddScoped<ShikiDataHelper>();

        var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? builder.Configuration["ApiBaseAddress"];

        builder.Services.AddHttpClient<ApiClientService>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
            //client.BaseAddress = new Uri(builder.Configuration.GetSection("Kestrel").GetSection("Endpoints").GetSection("Https")["Url"]);
            //client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"]);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        }).SetHandlerLifetime(TimeSpan.FromMinutes(1));
        builder.Services.AddSingleton<ApiClientService>();
        builder.Services.AddSingleton<SettingsService>();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo
                    { Title = "Roulette API", Version = "v1", Description = "An ASP.NET Core Web API for ROULETTE" });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
        });
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
            options.InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value;
        });
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        //TODO: !!!!!!!! причесать тут всё
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            //.AddIdentityCookies()
            .AddYandex(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Yandex:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Yandex:ClientSecret"];
                options.CallbackPath = new PathString("/Account/SingInYandex");
            })
            .AddIdentityCookies();
        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("AdminOnly", policy =>
        //        policy.RequireRole("admin"));
        //});
       
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

        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddScoped<GameService>();
        builder.Services.AddScoped<GameGenreService>();
        builder.Services.AddScoped<GameLanguageService>();
        builder.Services.AddScoped<UserChoiceHistoryService>();

        var app = builder.Build();
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
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
        app.UseAntiforgery();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapAdditionalIdentityEndpoints();
        app.Run();
    }
}