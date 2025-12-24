using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsData.Modules.Competitions.Migrations
{
    /// <inheritdoc />
    public partial class matchstats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchStats",
                schema: "competitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsHome = table.Column<bool>(type: "bit", nullable: false),
                    PossessionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shots = table.Column<int>(type: "int", nullable: false),
                    ShotsOnTarget = table.Column<int>(type: "int", nullable: false),
                    Corners = table.Column<int>(type: "int", nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false),
                    RedCards = table.Column<int>(type: "int", nullable: false),
                    Fouls = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchStats_MatchId",
                schema: "competitions",
                table: "MatchStats",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchStats",
                schema: "competitions");
        }
    }
}
