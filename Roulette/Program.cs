using System.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Roulette.Components;
using Roulette.Data;
using Roulette.Helpers;
using Roulette.Services;
using ShikimoriSharp;

namespace Roulette
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                options.DetailedErrors = true;
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            
            builder.Services.AddAntDesign();

            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();

            builder.Services.AddHttpClient<ShikimoriApiConnectorService>(client =>
            {
                string baseUrl = builder.Configuration.GetSection("Shikimori")["BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).SetHandlerLifetime(TimeSpan.FromMinutes(1)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
               // ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });//TODO настроить сертификаты
            builder.Services.AddSingleton <ShikimoriApiConnectorService> ();
            builder.Services.AddScoped<ShikiDataHelper>();

            var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? builder.Configuration["ApiBaseAddress"];

            builder.Services.AddHttpClient<ApiClientService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                //client.BaseAddress = new Uri(builder.Configuration.GetSection("Kestrel").GetSection("Endpoints").GetSection("Https")["Url"]);
                //client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            }).SetHandlerLifetime(TimeSpan.FromMinutes(1));
            builder.Services.AddSingleton<ApiClientService>();
            builder.Services.AddSingleton<SettingsService>();
           
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Roulette API", Version = "v1", Description = "An ASP.NET Core Web API for ROULETTE", });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename),true);
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            
            

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
                app.UseHsts();
            }
            //TODO попробовать с ENV переменной и таким вот перенаправлением
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.Run();
        }
    }
}
