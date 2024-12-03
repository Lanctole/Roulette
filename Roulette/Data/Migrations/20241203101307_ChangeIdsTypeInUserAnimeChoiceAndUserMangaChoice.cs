using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdsTypeInUserAnimeChoiceAndUserMangaChoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnimeChoices_Animes_AnimeId",
                table: "UserAnimeChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMangaChoices_Mangas_MangaId",
                table: "UserMangaChoices");

            migrationBuilder.DropIndex(
                name: "IX_UserMangaChoices_MangaId",
                table: "UserMangaChoices");

            migrationBuilder.DropIndex(
                name: "IX_UserAnimeChoices_AnimeId",
                table: "UserAnimeChoices");

            migrationBuilder.AlterColumn<int>(
                name: "MangaId",
                table: "UserMangaChoices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "AnimeId",
                table: "UserAnimeChoices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_UserMangaChoices_MangaId",
                table: "UserMangaChoices",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeChoices_AnimeId",
                table: "UserAnimeChoices",
                column: "AnimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnimeChoices_Animes_AnimeId",
                table: "UserAnimeChoices",
                column: "AnimeId",
                principalTable: "Animes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMangaChoices_Mangas_MangaId",
                table: "UserMangaChoices",
                column: "MangaId",
                principalTable: "Mangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_UserAnimeChoices_Animes_AnimeId",
               table: "UserAnimeChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMangaChoices_Mangas_MangaId",
                table: "UserMangaChoices");

            migrationBuilder.DropIndex(
                name: "IX_UserMangaChoices_MangaId",
                table: "UserMangaChoices");

            migrationBuilder.DropIndex(
                name: "IX_UserAnimeChoices_AnimeId",
                table: "UserAnimeChoices");

            migrationBuilder.AlterColumn<long>(
                name: "MangaId",
                table: "UserMangaChoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "AnimeId",
                table: "UserAnimeChoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_UserMangaChoices_MangaId",
                table: "UserMangaChoices",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeChoices_AnimeId",
                table: "UserAnimeChoices",
                column: "AnimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnimeChoices_Animes_AnimeId",
                table: "UserAnimeChoices",
                column: "AnimeId",
                principalTable: "Animes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMangaChoices_Mangas_MangaId",
                table: "UserMangaChoices",
                column: "MangaId",
                principalTable: "Mangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
