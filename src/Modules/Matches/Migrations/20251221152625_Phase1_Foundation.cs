using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsData.Modules.Matches.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_Foundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeagueId",
                schema: "matches",
                table: "Matches",
                newName: "SeasonId");

            migrationBuilder.CreateTable(
                name: "MatchStatistics",
                schema: "matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PossessionHome = table.Column<int>(type: "int", nullable: false),
                    PossessionAway = table.Column<int>(type: "int", nullable: false),
                    ShotsHome = table.Column<int>(type: "int", nullable: false),
                    ShotsAway = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_MatchStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchStatistics",
                schema: "matches");

            migrationBuilder.RenameColumn(
                name: "SeasonId",
                schema: "matches",
                table: "Matches",
                newName: "LeagueId");
        }
    }
}
