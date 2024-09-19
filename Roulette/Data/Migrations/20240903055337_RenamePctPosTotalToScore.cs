using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePctPosTotalToScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PctPosTotal",
                table: "Games",
                newName: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Games",
                newName: "PctPosTotal");
        }
    }
}
