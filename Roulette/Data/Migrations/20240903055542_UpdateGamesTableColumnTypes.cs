using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGamesTableColumnTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"AppID\" TYPE bigint USING \"AppID\"::text::bigint;");


            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"ReleaseDate\" TYPE date USING \"ReleaseDate\"::text::date;");


            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"SupportedLanguages\" TYPE jsonb USING \"SupportedLanguages\"::text::jsonb;");

            migrationBuilder.Sql(
                "UPDATE \"Games\" SET \"Genres\" = replace(\"Genres\", '''', '\"') WHERE \"Genres\" LIKE '%''%';");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"Genres\" TYPE jsonb USING \"Genres\"::text::jsonb;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"Score\" TYPE integer USING \"Score\"::text::integer;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"AppID\" TYPE text USING \"AppID\"::bigint::text;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"ReleaseDate\" TYPE text USING \"ReleaseDate\"::date::text;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"SupportedLanguages\" TYPE text USING \"SupportedLanguages\"::jsonb::text;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"Genres\" TYPE text USING \"Genres\"::jsonb::text;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Games\" ALTER COLUMN \"Score\" TYPE text USING \"Score\"::integer::text;");
        }
    }
}
