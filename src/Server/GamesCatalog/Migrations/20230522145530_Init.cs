using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesCatalog.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CoverUrl = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscordId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerInfoGame",
                columns: table => new
                {
                    PlayerInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerInfoGame", x => new { x.GameId, x.PlayerInfoId });
                    table.ForeignKey(
                        name: "FK_PlayerInfoGame_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerInfoGame_PlayerInfo_PlayerInfoId",
                        column: x => x.PlayerInfoId,
                        principalTable: "PlayerInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTimes",
                columns: table => new
                {
                    PlayerInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTimes", x => new { x.PlayerInfoId, x.StartTime, x.EndTime });
                    table.ForeignKey(
                        name: "FK_PlayerTimes_PlayerInfo_PlayerInfoId",
                        column: x => x.PlayerInfoId,
                        principalTable: "PlayerInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerInfoGame_PlayerInfoId",
                table: "PlayerInfoGame",
                column: "PlayerInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerInfoGame");

            migrationBuilder.DropTable(
                name: "PlayerTimes");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "PlayerInfo");
        }
    }
}
