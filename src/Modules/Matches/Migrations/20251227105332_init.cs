using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsData.Modules.Matches.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "matches_match",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SeasonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HomeTeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AwayTeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MatchDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    Venue = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches_match", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "matches_matchStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MatchId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PossessionHome = table.Column<int>(type: "int", nullable: false),
                    PossessionAway = table.Column<int>(type: "int", nullable: false),
                    ShotsHome = table.Column<int>(type: "int", nullable: false),
                    ShotsAway = table.Column<int>(type: "int", nullable: false),
                    ShotsOnTargetHome = table.Column<int>(type: "int", nullable: false),
                    ShotsOnTargetAway = table.Column<int>(type: "int", nullable: false),
                    CornersHome = table.Column<int>(type: "int", nullable: false),
                    CornersAway = table.Column<int>(type: "int", nullable: false),
                    FoulsHome = table.Column<int>(type: "int", nullable: false),
                    FoulsAway = table.Column<int>(type: "int", nullable: false),
                    YellowCardsHome = table.Column<int>(type: "int", nullable: false),
                    YellowCardsAway = table.Column<int>(type: "int", nullable: false),
                    RedCardsHome = table.Column<int>(type: "int", nullable: false),
                    RedCardsAway = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches_matchStatistics", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwayTeamId",
                table: "matches_match",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeTeamId",
                table: "matches_match",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_SeasonId_MatchDate",
                table: "matches_match",
                columns: new[] { "SeasonId", "MatchDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Match_Status",
                table: "matches_match",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "matches_matchStatistics",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches_match");

            migrationBuilder.DropTable(
                name: "matches_matchStatistics");
        }
    }
}
