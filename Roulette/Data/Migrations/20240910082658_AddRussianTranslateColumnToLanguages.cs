using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRussianTranslateColumnToLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Russian",
                table: "SupportedLanguages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Russian",
                table: "SupportedLanguages");
        }
    }
}
