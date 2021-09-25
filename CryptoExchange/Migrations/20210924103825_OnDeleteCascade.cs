using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoExchange.Migrations
{
    public partial class OnDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Coins_CoinId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Coins_CoinId",
                table: "Transactions",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Coins_CoinId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Coins_CoinId",
                table: "Transactions",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
