using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsData.Modules.ApiKeys.Migrations
{
    /// <inheritdoc />
    public partial class INIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "apikeys");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "apikeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plan = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RateLimitPerMinute = table.Column<int>(type: "int", nullable: false),
                    RateLimitPerDay = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiUsageLogs",
                schema: "apikeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    ResponseTimeMs = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUsageLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_IsActive",
                schema: "apikeys",
                table: "ApiKeys",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_KeyHash",
                schema: "apikeys",
                table: "ApiKeys",
                column: "KeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OwnerId",
                schema: "apikeys",
                table: "ApiKeys",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_ApiKeyId",
                schema: "apikeys",
                table: "ApiUsageLogs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_Timestamp",
                schema: "apikeys",
                table: "ApiUsageLogs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "apikeys");

            migrationBuilder.DropTable(
                name: "ApiUsageLogs",
                schema: "apikeys");
        }
    }
}
