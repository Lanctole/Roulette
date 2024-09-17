using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "BugReports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            
            migrationBuilder.Sql(@"
        UPDATE ""BugReports""
        SET ""StatusTemp"" = 
        CASE ""Status""
            WHEN 'Новый' THEN 0
            WHEN 'В процессе' THEN 1
            WHEN 'Решён' THEN 2
            WHEN 'Отложен' THEN 3
            ELSE 0
        END
    ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BugReports");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "BugReports",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BugReports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
        UPDATE ""BugReports""
        SET ""Status"" = 
        CASE ""StatusTemp""
            WHEN 0 THEN 'Новый'
            WHEN 1 THEN 'В процессе'
            WHEN 2 THEN 'Решён'
            WHEN 3 THEN 'Отложен'
            ELSE 'Новый'
        END
    ");

            migrationBuilder.DropColumn(
                name: "StatusTemp",
                table: "BugReports");
        }
    }
}
