using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsData.Modules.Competitions.Migrations
{
    /// <inheritdoc />
    public partial class addnewMatchestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                schema: "competitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwayTeamId",
                schema: "competitions",
                table: "Matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeTeamId",
                schema: "competitions",
                table: "Matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_SeasonId_MatchDate",
                schema: "competitions",
                table: "Matches",
                columns: new[] { "SeasonId", "MatchDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Match_Status",
                schema: "competitions",
                table: "Matches",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches",
                schema: "competitions");
        }
    }
}
