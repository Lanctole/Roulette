using Microsoft.EntityFrameworkCore;


namespace ShikimoriService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            builder.Services.AddHttpClient<GraphQLClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Shikimori:BaseUrl"]);
                client.DefaultRequestHeaders.Add("User-Agent", configuration["Auth:Application_name"]);
            }).AddHttpMessageHandler(() => new LoggingHandler());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
