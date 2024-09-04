using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropGenresAndLanguagesFromGamesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SupportedLanguages",
                table: "Games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "Games",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportedLanguages",
                table: "Games",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql(@"
            UPDATE Games g
            SET Genres = jsonb_agg(gen.""Name"")
            FROM GameGenre gg
            JOIN Genres gen ON gg.GenresId = gen.Id
            WHERE gg.GamesAppID = g.AppID
            GROUP BY g.AppID;");

            migrationBuilder.Sql(@"
            UPDATE Games g
            SET SupportedLanguages = jsonb_agg(lang.""Name"")
            FROM GameSupportedLanguage gsl
            JOIN SupportedLanguages lang ON gsl.SupportedLanguagesId = lang.Id
            WHERE gsl.GamesAppID = g.AppID
            GROUP BY g.AppID;");
        }
    }
}
