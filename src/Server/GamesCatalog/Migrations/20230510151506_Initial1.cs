using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesCatalog.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayerInfo_PlayerInfo_PlayerInfoId",
                table: "GamePlayerInfo");

            migrationBuilder.RenameColumn(
                name: "PlayerInfoId",
                table: "GamePlayerInfo",
                newName: "PlayersId");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlayerInfo_PlayerInfoId",
                table: "GamePlayerInfo",
                newName: "IX_GamePlayerInfo_PlayersId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayerInfo_PlayerInfo_PlayersId",
                table: "GamePlayerInfo",
                column: "PlayersId",
                principalTable: "PlayerInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayerInfo_PlayerInfo_PlayersId",
                table: "GamePlayerInfo");

            migrationBuilder.RenameColumn(
                name: "PlayersId",
                table: "GamePlayerInfo",
                newName: "PlayerInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlayerInfo_PlayersId",
                table: "GamePlayerInfo",
                newName: "IX_GamePlayerInfo_PlayerInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayerInfo_PlayerInfo_PlayerInfoId",
                table: "GamePlayerInfo",
                column: "PlayerInfoId",
                principalTable: "PlayerInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
