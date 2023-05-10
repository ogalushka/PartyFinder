using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesCatalog.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayerInfo");

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

            migrationBuilder.CreateTable(
                name: "GamePlayerInfo",
                columns: table => new
                {
                    GamesId = table.Column<int>(type: "int", nullable: false),
                    PlayersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayerInfo", x => new { x.GamesId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_GamePlayerInfo_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlayerInfo_PlayerInfo_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "PlayerInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayerInfo_PlayersId",
                table: "GamePlayerInfo",
                column: "PlayersId");
        }
    }
}
