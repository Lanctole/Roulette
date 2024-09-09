using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserChoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAnimeChoices",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AnimeId = table.Column<long>(type: "bigint", nullable: false),
                    ChosenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserAnimeChoices_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnimeChoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGameChoices",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    ChosenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserGameChoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGameChoices_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMangaChoices",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MangaId = table.Column<long>(type: "bigint", nullable: false),
                    ChosenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserMangaChoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMangaChoices_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRanobeChoices",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RanobeId = table.Column<long>(type: "bigint", nullable: false),
                    ChosenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserRanobeChoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRanobeChoices_Ranobes_RanobeId",
                        column: x => x.RanobeId,
                        principalTable: "Ranobes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeChoices_AnimeId",
                table: "UserAnimeChoices",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeChoices_UserId",
                table: "UserAnimeChoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGameChoices_GameId",
                table: "UserGameChoices",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGameChoices_UserId",
                table: "UserGameChoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMangaChoices_MangaId",
                table: "UserMangaChoices",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMangaChoices_UserId",
                table: "UserMangaChoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRanobeChoices_RanobeId",
                table: "UserRanobeChoices",
                column: "RanobeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRanobeChoices_UserId",
                table: "UserRanobeChoices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnimeChoices");

            migrationBuilder.DropTable(
                name: "UserGameChoices");

            migrationBuilder.DropTable(
                name: "UserMangaChoices");

            migrationBuilder.DropTable(
                name: "UserRanobeChoices");
        }
    }
}
