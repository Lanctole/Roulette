using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdsToUserChoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserRanobeChoices",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserMangaChoices",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserGameChoices",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserAnimeChoices",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRanobeChoices",
                table: "UserRanobeChoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMangaChoices",
                table: "UserMangaChoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGameChoices",
                table: "UserGameChoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnimeChoices",
                table: "UserAnimeChoices",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRanobeChoices",
                table: "UserRanobeChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMangaChoices",
                table: "UserMangaChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGameChoices",
                table: "UserGameChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnimeChoices",
                table: "UserAnimeChoices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRanobeChoices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserMangaChoices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserGameChoices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserAnimeChoices");
        }
    }
}
