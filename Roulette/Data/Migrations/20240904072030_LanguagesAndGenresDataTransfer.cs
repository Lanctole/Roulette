using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class LanguagesAndGenresDataTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO public.""Genres"" (""Name"")
        SELECT DISTINCT TRIM(BOTH '""' FROM value::text) AS ""Name""
        FROM public.""Games"", jsonb_array_elements_text(public.""Games"".""Genres"") AS value
        WHERE value IS NOT NULL
    ");

            migrationBuilder.Sql(@"
        INSERT INTO public.""SupportedLanguages"" (""Name"")
        SELECT DISTINCT TRIM(BOTH '""' FROM value::text) AS ""Name""
        FROM public.""Games"", jsonb_array_elements_text(public.""Games"".""SupportedLanguages"") AS value
        WHERE value IS NOT NULL
    ");

            migrationBuilder.Sql(@"
        INSERT INTO public.""GameGenre"" (""GamesAppID"", ""GenresId"")
        SELECT g.""AppID"", gen.""Id""
        FROM public.""Games"" g
        JOIN jsonb_array_elements_text(g.""Genres"") AS genText ON TRUE
        JOIN public.""Genres"" gen ON gen.""Name"" = TRIM(BOTH '""' FROM genText::text)
    ");

            migrationBuilder.Sql(@"
        INSERT INTO public.""GameSupportedLanguage"" (""GamesAppID"", ""SupportedLanguagesId"")
        SELECT g.""AppID"", lang.""Id""
        FROM public.""Games"" g
        JOIN jsonb_array_elements_text(g.""SupportedLanguages"") AS langText ON TRUE
        JOIN public.""SupportedLanguages"" lang ON lang.""Name"" = TRIM(BOTH '""' FROM langText::text)
    ");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
